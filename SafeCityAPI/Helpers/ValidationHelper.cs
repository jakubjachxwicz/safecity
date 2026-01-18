using System.Text.RegularExpressions;

namespace SafeCityAPI.Helpers;

/// <summary>
/// Pomocnik do walidacji danych wejściowych użytkownika.
/// Zawiera reguły walidacji hasła i nickname zgodne z aplikacją mobilną i webową.
/// </summary>
public static class ValidationHelper
{
    // Regex dla nickname: tylko litery (a-z, A-Z), cyfry (0-9) i podkreślenie (_)
    private static readonly Regex NicknameRegex = new Regex(@"^[a-zA-Z0-9_]+$", RegexOptions.Compiled);

    /// <summary>
    /// Waliduje hasło według zasad:
    /// - Minimum 10 znaków
    /// - Przynajmniej jedna mała litera
    /// - Przynajmniej jedna wielka litera
    /// - Przynajmniej jedna cyfra
    /// - Przynajmniej jeden znak specjalny
    /// </summary>
    /// <param name="password">Hasło do walidacji</param>
    /// <param name="errorMessage">Komunikat błędu jeśli walidacja nie powiodła się</param>
    /// <returns>True jeśli hasło jest prawidłowe, false w przeciwnym razie</returns>
    public static bool ValidatePassword(string password, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            errorMessage = "WEAK_PASSWORD: Password cannot be empty";
            return false;
        }

        if (password.Length < 10)
        {
            errorMessage = "WEAK_PASSWORD: Password must be at least 10 characters long";
            return false;
        }

        if (!password.Any(char.IsLower))
        {
            errorMessage = "WEAK_PASSWORD: Password must contain at least one lowercase letter";
            return false;
        }

        if (!password.Any(char.IsUpper))
        {
            errorMessage = "WEAK_PASSWORD: Password must contain at least one uppercase letter";
            return false;
        }

        if (!password.Any(char.IsDigit))
        {
            errorMessage = "WEAK_PASSWORD: Password must contain at least one digit";
            return false;
        }

        // Znaki specjalne: wszystko co nie jest literą ani cyfrą
        if (!password.Any(c => !char.IsLetterOrDigit(c)))
        {
            errorMessage = "WEAK_PASSWORD: Password must contain at least one special character";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// Waliduje nickname według zasad:
    /// - Długość od 3 do 20 znaków
    /// - Tylko litery (a-z, A-Z), cyfry (0-9) i podkreślenie (_)
    /// </summary>
    /// <param name="nickname">Nickname do walidacji</param>
    /// <param name="errorMessage">Komunikat błędu jeśli walidacja nie powiodła się</param>
    /// <returns>True jeśli nickname jest prawidłowy, false w przeciwnym razie</returns>
    public static bool ValidateNickname(string nickname, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(nickname))
        {
            errorMessage = "INVALID_NICKNAME: Nickname cannot be empty";
            return false;
        }

        if (nickname.Length < 3)
        {
            errorMessage = "INVALID_NICKNAME: Nickname must be at least 3 characters long";
            return false;
        }

        if (nickname.Length > 20)
        {
            errorMessage = "INVALID_NICKNAME: Nickname cannot exceed 20 characters";
            return false;
        }

        if (!NicknameRegex.IsMatch(nickname))
        {
            errorMessage = "INVALID_NICKNAME: Nickname can only contain letters (a-z, A-Z), digits (0-9), and underscores (_)";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// Waliduje adres email (podstawowa walidacja).
    /// </summary>
    /// <param name="email">Email do walidacji</param>
    /// <param name="errorMessage">Komunikat błędu jeśli walidacja nie powiodła się</param>
    /// <returns>True jeśli email jest prawidłowy, false w przeciwnym razie</returns>
    public static bool ValidateEmail(string email, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            errorMessage = "INVALID_EMAIL: Email cannot be empty";
            return false;
        }

        // Podstawowa walidacja - musi zawierać @ i domenę
        if (!email.Contains("@") || !email.Contains("."))
        {
            errorMessage = "INVALID_EMAIL: Email must be in valid format (e.g., user@example.com)";
            return false;
        }

        var parts = email.Split('@');
        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
        {
            errorMessage = "INVALID_EMAIL: Email must be in valid format (e.g., user@example.com)";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
    
    /// <summary>
    /// Lista dozwolonych kategorii zgłoszeń
    /// </summary>
    private static readonly HashSet<string> ValidCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        "traffic",
        "trash",
        "fight",
        "illegal_parking",
        "illega_gathering",
        "drone",
        "other"
    };

    /// <summary>
    /// Waliduje kategorię zgłoszenia.
    /// </summary>
    /// <param name="category">Kategoria do walidacji</param>
    /// <param name="errorMessage">Komunikat błędu jeśli walidacja nie powiodła się</param>
    /// <returns>True jeśli kategoria jest prawidłowa, false w przeciwnym razie</returns>
    public static bool ValidateCategory(string? category, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            errorMessage = string.Empty;
            return true; // Pusta kategoria = domyślnie "other"
        }

        if (!ValidCategories.Contains(category))
        {
            errorMessage = $"INVALID_CATEGORY: Category must be one of: {string.Join(", ", ValidCategories)}";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// Normalizuje kategorię (lowercase, domyślna wartość)
    /// </summary>
    public static string NormalizeCategory(string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return "other";
    
        var normalized = category.Trim().ToLowerInvariant();
        return ValidCategories.Contains(normalized) ? normalized : "other";
    }
}