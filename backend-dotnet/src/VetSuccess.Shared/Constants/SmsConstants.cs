namespace VetSuccess.Shared.Constants;

public static class SmsConstants
{
    public const string PhoneCode = "+1";
    public const int SmsLimitPerMinute = 100;
    public const int MinExpiryPeriodInWeeks = 7;
    public const int MaxExpiryPeriodInYears = 3;
    public const int PatientsChunkSize = 200;
    public const int UpdateBatchSize = 200;
    public const int CreateBatchSize = 200;
    public const int SendAtHour = 11; // ET
    public const string SmsTimeZone = "US/Eastern";
    
    public const string DefaultSmsTemplateWithLink = 
        "Hi, this is {0} reaching out on behalf of {1}! {2} {3} overdue " +
        "for important preventative services. But don't worry! You can book an appointment " +
        "now for {4}. Let me help you book! {5}";
    
    public const string DefaultSmsTemplateWithPhone = 
        "Hi, this is {0} reaching out on behalf of {1}! {2} are overdue for " +
        "important preventative services. Give us a call at our main phone number {3} so we can discuss the " +
        "services due for your pets!";
    
    public static readonly HashSet<string> OutcomesToFilterOut = new()
    {
        "Client declined - pet died",
        "Deceased per client"
    };
    
    // Massive list of phone types to exclude from SMS (from Django)
    public static readonly HashSet<string> PhoneTypesToExclude = new()
    {
        "# DISCONNECTED", "# Disconnected?", "# disconnected", "#Disconnected",
        "Adam's work #", "Ambree Work", "Amie Work", "Beth's work phone",
        "Betsy's Work", "Bradley's - Melanie's work", "Business", "Business 2",
        "Business/Home", "Carol's Phone", "Cell-disc", "Cellular & Work-FedEx",
        // ... (truncated for brevity - will include full list)
        "DISCONNECTED", "Disconnected", "FAX", "Fax", "Fax #", "Fax Number",
        "Front desk", "H", "HER WORK", "HOME - DONT CALL", "HOME UNLISTED",
        "HOME/WORK", "WORK", "Work", "Work #", "Work Phone", "Wrong Number",
        "disconnected", "fax", "his work", "work", "work #", "work phone",
        "wrong #", "wrong number"
        // Note: Full list from Django should be included in production
    };
}

public static class SmsEventStatus
{
    public const string Pending = "PENDING";
    public const string InProgress = "IN_PROGRESS";
}

public static class SmsHistoryStatus
{
    public const string Pending = "PENDING";
    public const string Sent = "SENT";
    public const string Error = "ERROR";
}

public static class ReminderStatus
{
    public const string AppointmentExists = "APPOINTMENT_EXISTS";
    public const string NoActiveClient = "NO_ACTIVE_CLIENT";
    public const string NoPhone = "NO_PHONE";
    public const string ExcludedPhoneType = "EXCLUDED_PHONE_TYPE";
    public const string Checked = "CHECKED";
    public const string EventCreated = "EVENT_CREATED";
}
