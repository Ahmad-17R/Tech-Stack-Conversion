namespace VetSuccess.Domain.Entities;

public class Practice : BaseCallCenterEntity
{
    public string PracticeOduId { get; set; } = null!;
    public string? ServerOduId { get; set; }
    public Server? Server { get; set; }
    
    public string? PracticeName { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? Phone { get; set; }
    
    public bool? HasPimsConnection { get; set; }
    public string? Pims { get; set; }
    public DateTime? LatestExtractorUpdated { get; set; }
    public DateTime? LatestTransaction { get; set; }
    public DateTime? ServerImportFinished { get; set; }
    public DateTime? PracticeUpdatedAt { get; set; }
    
    public bool IsArchived { get; set; }
    
    // Navigation properties
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public PracticeSettings? PracticeSettings { get; set; }
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    public ICollection<SMSHistory> SMSHistories { get; set; } = new List<SMSHistory>();
}
