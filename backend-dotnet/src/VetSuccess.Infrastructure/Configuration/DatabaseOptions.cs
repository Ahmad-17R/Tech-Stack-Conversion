using System.ComponentModel.DataAnnotations;

namespace VetSuccess.Infrastructure.Configuration;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    [Range(1, 1000)]
    public int MaxPoolSize { get; set; } = 100;

    [Range(1, 3600)]
    public int ConnectionLifetimeSeconds { get; set; } = 300; // 5 minutes

    [Range(1, 300)]
    public int CommandTimeoutSeconds { get; set; } = 30;

    public bool EnableSensitiveDataLogging { get; set; } = false;

    public bool EnableDetailedErrors { get; set; } = false;
}
