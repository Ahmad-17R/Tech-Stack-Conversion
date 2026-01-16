namespace VetSuccess.Application.Interfaces;

public interface ISmsSendingJob
{
    Task ExecuteAsync(string smsHistoryId, CancellationToken cancellationToken = default);
}
