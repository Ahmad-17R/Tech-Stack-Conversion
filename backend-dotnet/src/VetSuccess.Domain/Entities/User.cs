using Microsoft.AspNetCore.Identity;

namespace VetSuccess.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public Guid Uuid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
