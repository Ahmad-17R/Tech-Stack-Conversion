using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Infrastructure.Data;
using VetSuccess.Shared.Constants;
using VetSuccess.Shared.Extensions;
using VetSuccess.Shared.Utilities;

namespace VetSuccess.Infrastructure.Jobs;

public class SmsAggregationJob : ISmsAggregationJob
{
    private readonly VetSuccessDbContext _dbContext;
    private readonly ILogger<SmsAggregationJob> _logger;

    public SmsAggregationJob(
        VetSuccessDbContext dbContext,
        ILogger<SmsAggregationJob> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 900 })]
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting SMS aggregation job");

        try
        {
            // Get practices with SMS enabled
            var practices = await _dbContext.Practices
                .Include(p => p.PracticeSettings)
                .Where(p => p.PracticeSettings!.IsSmsMailingEnabled && !p.IsArchived)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Found {Count} practices with SMS enabled", practices.Count);

            foreach (var practice in practices)
            {
                await ProcessPracticeAsync(practice, cancellationToken);
            }

            _logger.LogInformation("SMS aggregation job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SMS aggregation job");
            throw;
        }
    }

    private async Task ProcessPracticeAsync(Practice practice, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing practice {PracticeId}", practice.PracticeOduId);

        var launchDate = practice.PracticeSettings?.LaunchDate;
        if (!launchDate.HasValue)
        {
            _logger.LogWarning("Practice {PracticeId} has no launch date, skipping", practice.PracticeOduId);
            return;
        }

        // Get date range for reminders
        var (startDate, endDate) = GetDateRange(launchDate.Value, practice.PracticeSettings!);

        // Query patients with overdue reminders
        var patients = await GetPatientsWithRemindersAsync(
            practice.PracticeOduId,
            startDate,
            endDate,
            cancellationToken);

        _logger.LogInformation("Found {Count} patients with reminders for practice {PracticeId}", 
            patients.Count, practice.PracticeOduId);

        var smsDataMap = new Dictionary<string, SmsData>();
        var remindersToUpdate = new List<Reminder>();

        foreach (var patient in patients)
        {
            await ProcessPatientAsync(
                patient,
                practice,
                smsDataMap,
                remindersToUpdate,
                cancellationToken);
        }

        // Bulk update reminders
        if (remindersToUpdate.Any())
        {
            await BulkUpdateRemindersAsync(remindersToUpdate, cancellationToken);
        }

        // Create SMS events and histories
        if (smsDataMap.Any())
        {
            await CreateSmsEntitiesAsync(smsDataMap, practice, cancellationToken);
        }
    }

    private async Task ProcessPatientAsync(
        Patient patient,
        Practice practice,
        Dictionary<string, SmsData> smsDataMap,
        List<Reminder> remindersToUpdate,
        CancellationToken cancellationToken)
    {
        // Load reminders for this patient
        var reminders = await _dbContext.Reminders
            .Where(r => r.PatientOduId == patient.PatientOduId 
                && r.PracticeOduId == practice.PracticeOduId
                && r.SMSStatus == null
                && r.ExtractorRemovedAt == null)
            .OrderByDescending(r => r.DateDue)
            .ToListAsync(cancellationToken);

        if (!reminders.Any())
        {
            return;
        }

        // Check if patient has upcoming appointment
        var hasUpcomingAppointment = await _dbContext.Appointments
            .AnyAsync(a => a.PatientOduId == patient.PatientOduId
                && !a.IsCanceledAppointment.GetValueOrDefault()
                && a.AppointmentDate >= reminders[0].DateDue!.Value.AddDays(-14)
                && a.ExtractorRemovedAt == null,
                cancellationToken);

        if (hasUpcomingAppointment)
        {
            UpdateReminders(reminders, ReminderStatus.AppointmentExists, remindersToUpdate);
            return;
        }

        // Get active client for this patient
        var client = await GetActiveClientForPatientAsync(patient.PatientOduId, cancellationToken);

        if (client == null)
        {
            UpdateReminders(reminders, ReminderStatus.NoActiveClient, remindersToUpdate);
            return;
        }

        // Get primary phone for client
        var phone = await _dbContext.Phones
            .Where(p => p.ClientOduId == client.ClientOduId
                && p.IsPreferred.GetValueOrDefault()
                && p.AppNumber != null
                && p.ExtractorRemovedAt == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (phone == null)
        {
            UpdateReminders(reminders, ReminderStatus.NoPhone, remindersToUpdate);
            return;
        }

        // Check if phone type is excluded
        if (SmsConstants.PhoneTypesToExclude.Contains(phone.PhoneType ?? ""))
        {
            UpdateReminders(reminders, ReminderStatus.ExcludedPhoneType, remindersToUpdate);
            return;
        }

        // Add to SMS data map
        if (!smsDataMap.ContainsKey(client.ClientOduId))
        {
            smsDataMap[client.ClientOduId] = new SmsData
            {
                ClientId = client.ClientOduId,
                PhoneNumber = phone.AppNumber!
            };
        }

        var smsData = smsDataMap[client.ClientOduId];
        var patientName = !string.IsNullOrEmpty(patient.PatientName) 
            ? System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(patient.PatientName.ToLower())
            : patient.PatientName;
        
        smsData.PatientNames.Add(patientName ?? "");
        smsData.Reminders.Add(reminders[0]);

        if (reminders.Count > 1)
        {
            smsData.HasMultipleReminders = true;
            // Mark additional reminders as checked
            UpdateReminders(reminders.Skip(1).ToList(), ReminderStatus.Checked, remindersToUpdate);
        }
    }

    private async Task<Client?> GetActiveClientForPatientAsync(string patientId, CancellationToken cancellationToken)
    {
        return await _dbContext.ClientPatientRelationships
            .Where(r => r.PatientOduId == patientId
                && r.ExtractorRemovedAt == null)
            .OrderByDescending(r => r.IsPreferred)
            .Select(r => r.Client)
            .Where(c => c != null
                && !c.PimsIsInactive.GetValueOrDefault()
                && !c.PimsIsDeleted.GetValueOrDefault()
                && !c.PimsHasSuspendedReminders.GetValueOrDefault()
                && c.IsHomePractice.GetValueOrDefault()
                && c.ExtractorRemovedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<List<Patient>> GetPatientsWithRemindersAsync(
        string practiceId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Patients
            .Where(p => p.PatientName != null
                && !p.PimsIsDeceased.GetValueOrDefault()
                && !p.PimsIsInactive.GetValueOrDefault()
                && !p.PimsIsDeleted.GetValueOrDefault()
                && !p.PimsHasSuspendedReminders.GetValueOrDefault()
                && !p.IsDeceased.GetValueOrDefault()
                && p.DeathDate == null
                && p.EuthanasiaDate == null
                && p.ExtractorRemovedAt == null
                && (p.OutcomeOduId == null || !SmsConstants.OutcomesToFilterOut.Contains(p.OutcomeOduId))
                && p.Reminders.Any(r => r.PracticeOduId == practiceId
                    && r.DateDue >= startDate
                    && r.DateDue <= endDate
                    && r.SMSStatus == null
                    && r.ExtractorRemovedAt == null))
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private (DateTime startDate, DateTime endDate) GetDateRange(DateTime launchDate, PracticeSettings settings)
    {
        var today = DateTime.UtcNow.Date;

        if (launchDate.Date == today)
        {
            // First launch - use configured date range or defaults
            if (settings.StartDateForLaunch.HasValue && settings.EndDateForLaunch.HasValue)
            {
                return (settings.StartDateForLaunch.Value, settings.EndDateForLaunch.Value);
            }
            else if (settings.StartDateForLaunch.HasValue)
            {
                return (settings.StartDateForLaunch.Value, today.AddWeeks(-SmsConstants.MinExpiryPeriodInWeeks));
            }
            else
            {
                return (
                    today.AddYears(-SmsConstants.MaxExpiryPeriodInYears),
                    today.AddWeeks(-SmsConstants.MinExpiryPeriodInWeeks)
                );
            }
        }
        else
        {
            // Regular run - only get reminders due exactly 7 weeks ago
            var targetDate = today.AddWeeks(-SmsConstants.MinExpiryPeriodInWeeks);
            return (targetDate, targetDate);
        }
    }

    private void UpdateReminders(
        IEnumerable<Reminder> reminders,
        string status,
        List<Reminder> remindersToUpdate)
    {
        foreach (var reminder in reminders)
        {
            reminder.SMSStatus = status;
            reminder.UpdatedAt = DateTime.UtcNow;
            remindersToUpdate.Add(reminder);
        }
    }

    private async Task BulkUpdateRemindersAsync(
        List<Reminder> reminders,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Bulk updating {Count} reminders", reminders.Count);

        // Process in batches
        for (int i = 0; i < reminders.Count; i += SmsConstants.UpdateBatchSize)
        {
            var batch = reminders.Skip(i).Take(SmsConstants.UpdateBatchSize);
            _dbContext.Reminders.UpdateRange(batch);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task CreateSmsEntitiesAsync(
        Dictionary<string, SmsData> smsDataMap,
        Practice practice,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating {Count} SMS events and histories", smsDataMap.Count);

        var smsEvents = new List<SMSEvent>();
        var smsHistories = new List<SMSHistory>();
        var remindersToUpdate = new List<Reminder>();

        foreach (var (clientId, smsData) in smsDataMap)
        {
            var smsHistoryId = Guid.NewGuid();
            var smsText = CreateSmsText(smsData, practice.PracticeSettings!);

            var context = new SMSContext
            {
                NumberFrom = practice.PracticeSettings!.SmsSendersPhone ?? "",
                NumberTo = smsData.PhoneNumber,
                PracticeId = practice.PracticeOduId,
                SmsHistoryId = smsHistoryId.ToString(),
                Text = smsText
            };

            var smsEvent = new SMSEvent
            {
                SendAt = GetSendAtTime(),
                Status = SmsEventStatus.Pending,
                Context = JsonSerializer.Serialize(context)
            };

            var smsHistory = new SMSHistory
            {
                Uuid = smsHistoryId,
                PracticeOduId = practice.PracticeOduId,
                ClientOduId = clientId,
                EventContext = JsonSerializer.Serialize(context),
                Status = SmsHistoryStatus.Pending
            };

            smsEvents.Add(smsEvent);
            smsHistories.Add(smsHistory);

            // Update reminders with SMS history reference
            foreach (var reminder in smsData.Reminders)
            {
                reminder.SMSHistoryId = smsHistoryId;
                reminder.SMSStatus = ReminderStatus.EventCreated;
                reminder.UpdatedAt = DateTime.UtcNow;
                remindersToUpdate.Add(reminder);
            }
        }

        // Bulk create
        await _dbContext.SMSEvents.AddRangeAsync(smsEvents, cancellationToken);
        await _dbContext.SMSHistories.AddRangeAsync(smsHistories, cancellationToken);
        _dbContext.Reminders.UpdateRange(remindersToUpdate);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created {EventCount} SMS events and {HistoryCount} SMS histories",
            smsEvents.Count, smsHistories.Count);
    }

    private string CreateSmsText(SmsData smsData, PracticeSettings settings)
    {
        var (yourPetsCapitalized, yourPets, beVerb) = ConcatenateNames(smsData.PatientNames);

        var template = smsData.HasMultipleReminders
            ? SmsConstants.DefaultSmsTemplateWithLink
            : GetTemplate(smsData.Reminders);

        return string.Format(
            template,
            settings.SmsScheduler ?? "",
            settings.SmsPracticeName ?? "",
            yourPetsCapitalized,
            beVerb,
            yourPets,
            settings.SmsLink ?? "",
            settings.SmsPhone ?? "");
    }

    private string GetTemplate(List<Reminder> reminders)
    {
        // Simplified template selection - in production, query SMSTemplate table
        // For now, use default template with link
        return SmsConstants.DefaultSmsTemplateWithLink;
    }

    private (string capitalized, string lowercase, string beVerb) ConcatenateNames(List<string> names)
    {
        if (names.Count == 1)
        {
            return (names[0], names[0], "is");
        }
        else if (names.Count <= 3)
        {
            var joined = string.Join(", ", names.Take(names.Count - 1)) + $" and {names.Last()}";
            return (joined, joined, "are");
        }
        else
        {
            return ("Your pets", "your pets", "are");
        }
    }

    private DateTime GetSendAtTime()
    {
        // Get next workday at 11 AM ET
        var nextWorkday = USHolidays.GetNextWorkday(DateTime.Now.AddDays(-1));
        var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var sendTime = new DateTime(
            nextWorkday.Year,
            nextWorkday.Month,
            nextWorkday.Day,
            SmsConstants.SendAtHour,
            0,
            0,
            DateTimeKind.Unspecified);

        return TimeZoneInfo.ConvertTimeToUtc(sendTime, easternZone);
    }

    private class SmsData
    {
        public string ClientId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<string> PatientNames { get; set; } = new();
        public List<Reminder> Reminders { get; set; } = new();
        public bool HasMultipleReminders { get; set; }
    }
}
