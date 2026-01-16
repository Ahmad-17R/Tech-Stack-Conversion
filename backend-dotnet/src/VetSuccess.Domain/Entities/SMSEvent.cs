using System.Text.Json;

namespace VetSuccess.Domain.Entities;

public class SMSEvent : BaseEntity
{
    public DateTime SendAt { get; set; }
    public string Status { get; set; } = "PENDING"; // PENDING, IN_PROGRESS
    public string Context { get; set; } = string.Empty; // JSON serialized SMSContext
    
    public SMSContext GetContext()
    {
        return JsonSerializer.Deserialize<SMSContext>(Context) ?? new SMSContext();
    }
    
    public void SetContext(SMSContext context)
    {
        Context = JsonSerializer.Serialize(context);
    }
}

public class SMSContext
{
    public string NumberFrom { get; set; } = string.Empty;
    public string NumberTo { get; set; } = string.Empty;
    public string PracticeId { get; set; } = string.Empty;
    public string SmsHistoryId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
