namespace VetSuccess.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Uuid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
