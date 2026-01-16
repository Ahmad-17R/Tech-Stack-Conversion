namespace VetSuccess.Application.DTOs.Practice;

public class PracticeDto
{
    public string PracticeOduId { get; set; } = string.Empty;
    public Guid Uuid { get; set; }
    public string? PracticeName { get; set; }
    public string? ServerOduId { get; set; }
    public string? ServerName { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public PracticeSettingsDto? Settings { get; set; }
}

public class PracticeSettingsDto
{
    public Guid Uuid { get; set; }
    public string? SchedulerType { get; set; }
    public int? DaysBeforeAppointment { get; set; }
    public int? DaysAfterAppointment { get; set; }
    public bool? EnableSmsReminders { get; set; }
    public bool? EnableEmailReminders { get; set; }
    public string? SmsSendersPhone { get; set; }
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
