namespace VetSuccess.Domain.Entities;

public class Address : BaseCallCenterEntity
{
    public string AddressOduId { get; set; } = null!;
    public string? ServerOduId { get; set; }
    public Server? Server { get; set; }
    public string? ClientOduId { get; set; }
    public Client? Client { get; set; }
    
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? AddressType { get; set; }
    public bool? IsPreferred { get; set; }
}
