namespace VetSuccess.Application.DTOs.Admin;

public class PracticeSettingsAdminDto
{
    public Guid Uuid { get; set; }
    public string PracticeOduId { get; set; } = null!;
    public bool IsSmsMailingEnabled { get; set; }
    public string? SmsSendersPhone { get; set; }
    public string? SmsPracticeName { get; set; }
    public string? SmsScheduler { get; set; }
    public string? SmsLink { get; set; }
    public string? SmsPhone { get; set; }
    public bool IsEmailUpdatesEnabled { get; set; }
    public string? Email { get; set; }
    public string? SchedulerEmail { get; set; }
    public string? RdoName { get; set; }
    public string? RdoEmail { get; set; }
    public DateTime? LaunchDate { get; set; }
    public DateTime? StartDateForLaunch { get; set; }
    public DateTime? EndDateForLaunch { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreatePracticeSettingsRequest
{
    public string PracticeOduId { get; set; } = null!;
    public bool IsSmsMailingEnabled { get; set; }
    public string? SmsSendersPhone { get; set; }
    public string? SmsPracticeName { get; set; }
    public string? SmsScheduler { get; set; }
    public string? SmsLink { get; set; }
    public string? SmsPhone { get; set; }
    public bool IsEmailUpdatesEnabled { get; set; }
    public string? Email { get; set; }
    public string? SchedulerEmail { get; set; }
    public string? RdoName { get; set; }
    public string? RdoEmail { get; set; }
    public DateTime? LaunchDate { get; set; }
    public DateTime? StartDateForLaunch { get; set; }
    public DateTime? EndDateForLaunch { get; set; }
}

public class UpdatePracticeSettingsRequest
{
    public bool? IsSmsMailingEnabled { get; set; }
    public string? SmsSendersPhone { get; set; }
    public string? SmsPracticeName { get; set; }
    public string? SmsScheduler { get; set; }
    public string? SmsLink { get; set; }
    public string? SmsPhone { get; set; }
    public bool? IsEmailUpdatesEnabled { get; set; }
    public string? Email { get; set; }
    public string? SchedulerEmail { get; set; }
    public string? RdoName { get; set; }
    public string? RdoEmail { get; set; }
    public DateTime? LaunchDate { get; set; }
    public DateTime? StartDateForLaunch { get; set; }
    public DateTime? EndDateForLaunch { get; set; }
}
