namespace VetSuccess.Domain.Entities;

public class SMSHistory : BaseEntity
{
    public string? ClientOduId { get; set; }
    public Client? Client { get; set; }
    public string? PracticeOduId { get; set; }
    public Practice? Practice { get; set; }
    
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public DateTime? SentAt { get; set; }
    public bool IsFollowed { get; set; }
    public string? MessageText { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EventContext { get; set; } // JSON serialized context
    public string? Response { get; set; } // JSON response from Dialpad
    public string? ErrorMessage { get; set; }
    
    // Navigation properties
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
}
