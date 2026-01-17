using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace SafeCityAPI.Helpers;

/// <summary>
/// Bezpieczne hashowanie haseł przy użyciu Argon2id.
/// Zgodne z OWASP guidelines dla 2024.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashuje hasło używając Argon2id z unikalną solą.
    /// </summary>
    string HashPassword(string password);
    
    /// <summary>
    /// Weryfikuje hasło porównując z hashem.
    /// </summary>
    bool VerifyPassword(string password, string hash);
}

public class Argon2PasswordHasher : IPasswordHasher
{
    // Parametry Argon2id - zoptymalizowane dla bezpieczeństwa vs performance
    private const int SaltSize = 16;           // 128 bitów (16 bajtów)
    private const int HashSize = 32;           // 256 bitów (32 bajty)
    private const int Iterations = 4;          // Liczba iteracji (OWASP: 3-4)
    private const int MemorySize = 65536;      // 64 MB pamięci (OWASP: 46 MB minimum)
    private const int DegreeOfParallelism = 1; // Liczba wątków (1 dla serwerów)

    /// <summary>
    /// Hashuje hasło używając Argon2id.
    /// Format: $argon2id$v=19$m=65536,t=4,p=1$[salt]$[hash]
    /// </summary>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        // Generuj losową sól (cryptographically secure)
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        
        // Hashuj hasło z Argon2id
        byte[] hash = HashPasswordInternal(password, salt);
        
        // Format: $argon2id$v=19$m=65536,t=4,p=1$[base64_salt]$[base64_hash]
        return $"$argon2id$v=19$m={MemorySize},t={Iterations},p={DegreeOfParallelism}$" +
               $"{Convert.ToBase64String(salt)}$" +
               $"{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Weryfikuje hasło porównując z hashem.
    /// Odporny na timing attacks (constant-time comparison).
    /// </summary>
    public bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (string.IsNullOrWhiteSpace(storedHash))
            return false;

        try
        {
            // Parse stored hash
            var parts = storedHash.Split('$');
            
            // Validate format: $argon2id$v=19$m=X,t=Y,p=Z$salt$hash
            if (parts.Length != 6 || parts[1] != "argon2id")
                return false;

            // Extract salt and hash
            byte[] salt = Convert.FromBase64String(parts[4]);
            byte[] storedHashBytes = Convert.FromBase64String(parts[5]);

            // Hash provided password with same salt
            byte[] computedHash = HashPasswordInternal(password, salt);

            // Constant-time comparison (odporny na timing attacks)
            return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
        }
        catch
        {
            // Jeśli parsing się nie uda, zwróć false (nie ujawniaj szczegółów błędu)
            return false;
        }
    }

    /// <summary>
    /// Wewnętrzna metoda hashująca z Argon2id.
    /// </summary>
    private byte[] HashPasswordInternal(string password, byte[] salt)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = DegreeOfParallelism,
            MemorySize = MemorySize,
            Iterations = Iterations
        };

        return argon2.GetBytes(HashSize);
    }
}
