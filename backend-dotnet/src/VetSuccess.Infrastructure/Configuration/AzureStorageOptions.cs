using System.ComponentModel.DataAnnotations;

namespace VetSuccess.Infrastructure.Configuration;

public class AzureStorageOptions
{
    public const string SectionName = "AzureStorage";

    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    [Required]
    public string ContainerName { get; set; } = "vetsuccess";

    [Range(1000, 300000)]
    public int TimeoutMilliseconds { get; set; } = 60000;

    public bool CreateContainerIfNotExists { get; set; } = true;
}
