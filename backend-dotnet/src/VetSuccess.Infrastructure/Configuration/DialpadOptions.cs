using System.ComponentModel.DataAnnotations;

namespace VetSuccess.Infrastructure.Configuration;

public class DialpadOptions
{
    public const string SectionName = "Dialpad";

    [Required]
    public string ApiToken { get; set; } = string.Empty;

    [Required]
    [Url]
    public string BaseUrl { get; set; } = "https://dialpad.com/api/v2/";

    public bool SendSms { get; set; } = true;

    [Range(1000, 60000)]
    public int TimeoutMilliseconds { get; set; } = 30000;

    [Range(0, 5)]
    public int MaxRetryAttempts { get; set; } = 3;
}
