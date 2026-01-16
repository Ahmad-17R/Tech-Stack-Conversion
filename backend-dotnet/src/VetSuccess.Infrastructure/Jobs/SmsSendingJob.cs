using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Infrastructure.Data;
using VetSuccess.Shared.Constants;

namespace VetSuccess.Infrastructure.Jobs;

public class SmsSendingJob : ISmsSendingJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDialpadService _dialpadService;
    private readonly VetSuccessDbContext _dbContext;
    private readonly ILogger<SmsSendingJob> _logger;

    public SmsSendingJob(
        IUnitOfWork unitOfWork,
        IDialpadService dialpadService,
        VetSuccessDbContext dbContext,
        ILogger<SmsSendingJob> logger)
    {
        _unitOfWork = unitOfWork;
        _dialpadService = dialpadService;
        _dbContext = dbContext;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 900 })]
    public async Task ExecuteAsync(string smsEventId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting SMS sending job for event ID: {EventId}", smsEventId);

        var eventGuid = Guid.Parse(smsEventId);
        var smsEvent = await _dbContext.SMSEvents
            .FirstOrDefaultAsync(e => e.Uuid == eventGuid, cancellationToken);

        if (smsEvent == null)
        {
            _logger.LogWarning("SMS event {EventId} not found", smsEventId);
            return;
        }

        var context = smsEvent.GetContext();
        var now = DateTime.UtcNow;
        var shouldDeleteEvent = true;

        try
        {
            // Send SMS via Dialpad
            var result = await _dialpadService.SendSmsAsync(
                context.NumberTo,
                context.Text,
                cancellationToken);

            if (result.IsSuccess)
            {
                // Update history as SENT
                await UpdateHistoryRecordAsync(
                    context.SmsHistoryId,
                    SmsHistoryStatus.Sent,
                    now,
                    sentAt: now,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("Successfully sent SMS {HistoryId}", context.SmsHistoryId);
            }
            else if (result.IsSkipped)
            {
                // SMS sending is disabled
                await UpdateHistoryRecordAsync(
                    context.SmsHistoryId,
                    SmsHistoryStatus.Pending,
                    now,
                    errorMessage: "SMS sending is disabled",
                    cancellationToken: cancellationToken);

                _logger.LogInformation("SMS sending skipped for {HistoryId}", context.SmsHistoryId);
            }
            else
            {
                // Error occurred
                await UpdateHistoryRecordAsync(
                    context.SmsHistoryId,
                    SmsHistoryStatus.Error,
                    now,
                    errorMessage: result.ErrorMessage,
                    cancellationToken: cancellationToken);

                _logger.LogError("Failed to send SMS {HistoryId}: {Error}", 
                    context.SmsHistoryId, result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SMS sending job for event ID: {EventId}", smsEventId);
            
            await UpdateHistoryRecordAsync(
                context.SmsHistoryId,
                SmsHistoryStatus.Error,
                now,
                errorMessage: ex.Message,
                cancellationToken: cancellationToken);
            
            throw;
        }
        finally
        {
            // Delete the event after processing
            if (shouldDeleteEvent)
            {
                _dbContext.SMSEvents.Remove(smsEvent);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private async Task UpdateHistoryRecordAsync(
        string historyId,
        string status,
        DateTime updatedAt,
        DateTime? sentAt = null,
        string? errorMessage = null,
        CancellationToken cancellationToken = default)
    {
        var historyGuid = Guid.Parse(historyId);
        var history = await _dbContext.SMSHistories
            .FirstOrDefaultAsync(h => h.Uuid == historyGuid, cancellationToken);

        if (history != null)
        {
            history.Status = status;
            history.UpdatedAt = updatedAt;
            
            if (sentAt.HasValue)
            {
                history.SentAt = sentAt.Value;
            }
            
            if (errorMessage != null)
            {
                history.ErrorMessage = errorMessage;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
