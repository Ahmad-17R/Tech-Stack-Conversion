using System.ComponentModel.DataAnnotations;

namespace VetSuccess.Infrastructure.Configuration;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(60, 86400)] // 1 minute to 24 hours
    public int AccessTokenLifetimeSeconds { get; set; } = 3600; // 1 hour default

    [Range(3600, 2592000)] // 1 hour to 30 days
    public int RefreshTokenLifetimeSeconds { get; set; } = 604800; // 7 days default
}
