namespace Handlers;

public class BcryptHandler
{
    public string GenerateHashPass(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }

    public bool VerifyHashPass(string password, string hashPass)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password,hashPass);
    }

}