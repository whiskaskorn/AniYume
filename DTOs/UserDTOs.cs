namespace DTOs;

public class UserRegistrationDTO
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class UserLoginDTO
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class UserConfirmEmailDTO
{
    public string? Email { get; set; }
    public int Code { get; set; }
}

public class UserPutDTO
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}