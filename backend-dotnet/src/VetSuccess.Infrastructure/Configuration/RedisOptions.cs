using System.ComponentModel.DataAnnotations;

namespace VetSuccess.Infrastructure.Configuration;

public class RedisOptions
{
    public const string SectionName = "Redis";

    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    public string InstanceName { get; set; } = "VetSuccess:";

    [Range(1, 3600)]
    public int DefaultExpirationSeconds { get; set; } = 3600; // 1 hour

    [Range(1, 10)]
    public int ConnectRetry { get; set; } = 3;

    [Range(1000, 30000)]
    public int ConnectTimeoutMilliseconds { get; set; } = 5000;

    [Range(1000, 30000)]
    public int SyncTimeoutMilliseconds { get; set; } = 5000;

    public bool AbortOnConnectFail { get; set; } = false;
}
