namespace VetSuccess.Application.DTOs.Auth;

public class RegisterResponse
{
    public string Message { get; set; } = null!;
    public UserInfo User { get; set; } = null!;
}

public class UserInfo
{
    public Guid Uuid { get; set; }
    public string Email { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
