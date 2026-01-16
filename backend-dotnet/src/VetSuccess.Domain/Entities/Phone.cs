namespace VetSuccess.Domain.Entities;

public class Phone : BaseCallCenterEntity
{
    public string PhoneOduId { get; set; } = null!;
    public string? ServerOduId { get; set; }
    public Server? Server { get; set; }
    public string? ClientOduId { get; set; }
    public Client? Client { get; set; }
    
    public string? PhoneNumber { get; set; }
    public string? PhoneType { get; set; }
    public bool? IsPreferred { get; set; }
    public string? AppNumber { get; set; }
}
