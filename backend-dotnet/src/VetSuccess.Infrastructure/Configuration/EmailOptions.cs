using System.ComponentModel.DataAnnotations;

namespace VetSuccess.Infrastructure.Configuration;

public class EmailOptions
{
    public const string SectionName = "Email";

    [Required]
    public string SendGridApiKey { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string DefaultFromEmail { get; set; } = string.Empty;

    [Required]
    public string DefaultFromName { get; set; } = "VetSuccess";

    public bool UseDebugEmail { get; set; } = false;

    [EmailAddress]
    public string? DebugRecipient { get; set; }

    [EmailAddress]
    public string? DebugCcRecipient { get; set; }

    [Range(1000, 60000)]
    public int TimeoutMilliseconds { get; set; } = 30000;
}
