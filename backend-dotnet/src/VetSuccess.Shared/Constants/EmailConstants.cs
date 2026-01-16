namespace VetSuccess.Shared.Constants;

public static class EmailConstants
{
    public const string DateFormatToGetFiles = "yyyy/MM/dd";
    public const string FileExtension = ".xlsx";
    public const string FilenamePrefix = "Remote_Scheduling";
    public const string FilenameDateFormat = "MMddyyyy";
    public const string MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    
    public const string DailyUpdatesBody = @"
Hello,
Please find attached the file{0} with daily updates and changes to be made within your Practice Management System.
Thanks.
";
    
    public const string DailyUpdatesSubject = "Daily updates from Remote Scheduling team";
}

public static class UpdatesEmailEventStatus
{
    public const string NoFiles = "NO_FILES";
    public const string Pending = "PENDING";
    public const string Sent = "SENT";
    public const string Error = "ERROR";
}
