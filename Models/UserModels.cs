

namespace Models;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? HashPassword { get; set; }
    public bool isExsistsEmail { get; set; } = false;
    public int? CodeForConfirmEmail { get; set; } = null;
}