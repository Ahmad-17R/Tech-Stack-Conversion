namespace VetSuccess.Domain.Entities;

public class PracticeSettings : BaseEntity
{
    public string PracticeOduId { get; set; } = null!;
    public Practice Practice { get; set; } = null!;
    
    public bool IsSmsMailingEnabled { get; set; }
    public bool IsEmailUpdatesEnabled { get; set; }
    public string? SmsSendersPhone { get; set; }
    public string? SmsScheduler { get; set; }
    public string? SmsPracticeName { get; set; }
    public string? SmsPhone { get; set; }
    public string? SmsLink { get; set; }
    public string? Email { get; set; }
    public DateTime? LaunchDate { get; set; }
    public DateTime? StartDateForLaunch { get; set; }
    public DateTime? EndDateForLaunch { get; set; }
    public string? SchedulerEmail { get; set; }
    public string? RdoName { get; set; }
    public string? RdoEmail { get; set; }
}
