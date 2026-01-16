namespace VetSuccess.Domain.Entities;

public abstract class BaseCallCenterEntity : BaseEntity
{
    public DateTime? ExtractorRemovedAt { get; set; }
    public string? DataSource { get; set; }
}
