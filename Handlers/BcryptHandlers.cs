namespace Handlers;

public class BcryptHandler
{
    public string GenerateHashPass(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }

}