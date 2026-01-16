namespace VetSuccess.Domain.Entities;

public class Email : BaseCallCenterEntity
{
    public string EmailOduId { get; set; } = null!;
    public string? ServerOduId { get; set; }
    public Server? Server { get; set; }
    public string? ClientOduId { get; set; }
    public Client? Client { get; set; }
    
    public string? EmailType { get; set; }
    public string? EmailAddress { get; set; }
    public bool? IsPreferred { get; set; }
}
