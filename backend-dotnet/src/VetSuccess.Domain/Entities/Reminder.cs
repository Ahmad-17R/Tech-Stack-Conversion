namespace VetSuccess.Domain.Entities;

public class Reminder : BaseCallCenterEntity
{
    public string ReminderOduId { get; set; } = null!;
    public string? PatientOduId { get; set; }
    public Patient? Patient { get; set; }
    public string? ClientOduId { get; set; }
    public string? PracticeOduId { get; set; }
    public Guid? SMSHistoryId { get; set; }
    public SMSHistory? SMSHistory { get; set; }
    
    public DateTime? DateDue { get; set; }
    public string? Description { get; set; }
    public string? SMSStatus { get; set; }
}
