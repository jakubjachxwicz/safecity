namespace SafeCityMobile.Common;

public class Validator
{
    public bool ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public bool ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 10)
            return false;

        bool hasLower = false;
        bool hasUpper = false;
        bool hasDigit = false;
        bool hasSpecial = false;

        foreach (char c in password)
        {
            if (char.IsLower(c)) hasLower = true;
            else if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else hasSpecial = true;
        }

        return hasLower && hasUpper && hasDigit && hasSpecial;
    }
}