using Azure.Storage.Blobs;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Entities;
using VetSuccess.Infrastructure.Data;
using VetSuccess.Shared.Constants;
using VetSuccess.Shared.Utilities;

namespace VetSuccess.Infrastructure.Jobs;

public class DailyEmailUpdatesJob : IDailyEmailUpdatesJob
{
    private readonly VetSuccessDbContext _dbContext;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DailyEmailUpdatesJob> _logger;

    public DailyEmailUpdatesJob(
        VetSuccessDbContext dbContext,
        IBlobStorageService blobStorageService,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<DailyEmailUpdatesJob> logger)
    {
        _dbContext = dbContext;
        _blobStorageService = blobStorageService;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 900 })]
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting daily email updates job");

        try
        {
            // Check if today is a workday
            if (!IsWorkDay())
            {
                _logger.LogInformation("Today is not a working day, skipping daily email updates");
                return;
            }

            // Get servers with practices that have email updates enabled
            var servers = await GetServersWithEmailEnabledPracticesAsync(cancellationToken);
            
            if (!servers.Any())
            {
                _logger.LogInformation("No servers with email-enabled practices found");
                return;
            }

            // Get file paths from blob storage
            var serverFilesMap = await GetServerFilesMapAsync(cancellationToken);

            // Create events and send emails
            foreach (var server in servers)
            {
                var filePaths = serverFilesMap.GetValueOrDefault(server.ServerOduId, new List<string>());
                
                foreach (var practice in server.Practices.Where(p => p.PracticeSettings?.IsEmailUpdatesEnabled == true))
                {
                    var eventEntity = await CreateEventAsync(practice.PracticeOduId, filePaths, cancellationToken);
                    
                    if (filePaths.Any())
                    {
                        // Queue the email sending
                        BackgroundJob.Enqueue<DailyEmailUpdatesJob>(job => 
                            job.SendDailyUpdatesEmailAsync(eventEntity.Uuid.ToString(), cancellationToken));
                    }
                }
            }

            _logger.LogInformation("Daily email updates job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in daily email updates job");
            throw;
        }
    }

    public async Task SendDailyUpdatesEmailAsync(string eventId, CancellationToken cancellationToken = default)
    {
        var eventGuid = Guid.Parse(eventId);
        var emailEvent = await _dbContext.UpdatesEmailEvents
            .Include(e => e.Practice)
            .ThenInclude(p => p!.PracticeSettings)
            .FirstOrDefaultAsync(e => e.Uuid == eventGuid, cancellationToken);

        if (emailEvent == null)
        {
            _logger.LogWarning("Email event {EventId} not found", eventId);
            return;
        }

        try
        {
            var practiceSettings = emailEvent.Practice?.PracticeSettings;
            if (practiceSettings == null || string.IsNullOrEmpty(practiceSettings.Email))
            {
                throw new Exception("Invalid practice email configuration");
            }

            var attachments = new List<EmailAttachment>();

            // Download files from blob storage and create attachments
            foreach (var filePath in emailEvent.FilePaths)
            {
                var fileName = GetFileNameFromPath(filePath);
                var fileData = await DownloadFileFromBlobAsync(filePath, cancellationToken);
                
                attachments.Add(new EmailAttachment
                {
                    Filename = fileName,
                    Content = fileData,
                    MimeType = EmailConstants.MimeType
                });
            }

            // Prepare CC emails
            var ccEmails = new List<string>();
            if (!string.IsNullOrEmpty(practiceSettings.RdoEmail))
            {
                ccEmails.Add(practiceSettings.RdoEmail);
            }
            if (!string.IsNullOrEmpty(practiceSettings.SchedulerEmail))
            {
                ccEmails.Add(practiceSettings.SchedulerEmail);
            }

            // Send email
            await _emailService.SendDailyUpdatesAsync(
                practiceSettings.Email,
                attachments,
                ccEmails,
                cancellationToken);

            // Update event status
            emailEvent.Status = UpdatesEmailEventStatus.Sent;
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Daily updates email sent successfully for event {EventId}", eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending daily updates email for event {EventId}", eventId);
            
            emailEvent.Status = UpdatesEmailEventStatus.Error;
            emailEvent.ErrorMessage = ex.Message;
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            throw;
        }
    }

    private static bool IsWorkDay()
    {
        var today = DateTime.Now.Date;
        return !USHolidays.IsWeekend(today) && !USHolidays.IsHoliday(today);
    }

    private async Task<List<Server>> GetServersWithEmailEnabledPracticesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Servers
            .Include(s => s.Practices.Where(p => p.PracticeSettings!.IsEmailUpdatesEnabled && !p.IsArchived))
            .ThenInclude(p => p.PracticeSettings)
            .Where(s => s.Practices.Any(p => p.PracticeSettings!.IsEmailUpdatesEnabled))
            .ToListAsync(cancellationToken);
    }

    private async Task<UpdatesEmailEvent> CreateEventAsync(
        string practiceId, 
        List<string> filePaths, 
        CancellationToken cancellationToken)
    {
        var status = filePaths.Any() ? UpdatesEmailEventStatus.Pending : UpdatesEmailEventStatus.NoFiles;
        
        var eventEntity = new UpdatesEmailEvent
        {
            Status = status,
            FilePaths = filePaths,
            PracticeId = practiceId
        };

        _dbContext.UpdatesEmailEvents.Add(eventEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return eventEntity;
    }

    private async Task<Dictionary<string, List<string>>> GetServerFilesMapAsync(CancellationToken cancellationToken)
    {
        var serverFilesMap = new Dictionary<string, List<string>>();
        var containerName = _configuration["AzureStorage:UpdatesContainerName"] ?? "updates";
        var pathPrefix = _configuration["AzureStorage:UpdatesPathPrefix"] ?? "updates";

        try
        {
            var dateRangeToCheck = GetDateRangeToCheckFiles();
            
            foreach (var date in dateRangeToCheck)
            {
                var prefix = $"{pathPrefix}/{date}";
                var files = await _blobStorageService.ListFilesAsync(containerName, prefix, cancellationToken);
                
                foreach (var filePath in files)
                {
                    if (IsValidFilePath(filePath))
                    {
                        var parts = filePath.Split('/');
                        if (parts.Length >= 2)
                        {
                            var serverId = parts[^2]; // Second to last part is server ID
                            
                            if (!serverFilesMap.ContainsKey(serverId))
                            {
                                serverFilesMap[serverId] = new List<string>();
                            }
                            
                            serverFilesMap[serverId].Add(filePath);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting server files map from blob storage");
        }

        return serverFilesMap;
    }

    private static List<string> GetDateRangeToCheckFiles()
    {
        var previousWorkday = USHolidays.GetPreviousWorkday(DateTime.Now);
        var yesterday = DateTime.Now.AddDays(-1);
        
        var dates = new List<string>();
        var currentDate = previousWorkday;
        
        while (currentDate.Date <= yesterday.Date)
        {
            dates.Add(currentDate.ToString(EmailConstants.DateFormatToGetFiles));
            currentDate = currentDate.AddDays(1);
        }
        
        return dates;
    }

    private static bool IsValidFilePath(string path)
    {
        var parts = path.Split('/');
        return parts.Length == 6 && parts[^1].EndsWith(EmailConstants.FileExtension);
    }

    private static string GetFileNameFromPath(string filePath)
    {
        var parts = filePath.Split('/');
        var dateStr = string.Join("/", parts[1..4]); // Get YYYY/MM/DD
        var date = DateTime.ParseExact(dateStr, EmailConstants.DateFormatToGetFiles, null);
        return $"{EmailConstants.FilenamePrefix}_{date.ToString(EmailConstants.FilenameDateFormat)}{EmailConstants.FileExtension}";
    }

    private async Task<byte[]> DownloadFileFromBlobAsync(string filePath, CancellationToken cancellationToken)
    {
        var containerName = _configuration["AzureStorage:UpdatesContainerName"] ?? "updates";
        return await _blobStorageService.DownloadFileBytesAsync(containerName, filePath, cancellationToken);
    }
}
