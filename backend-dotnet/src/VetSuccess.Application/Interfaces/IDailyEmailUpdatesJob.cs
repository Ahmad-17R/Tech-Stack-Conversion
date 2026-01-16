namespace VetSuccess.Application.Interfaces;

public interface IDailyEmailUpdatesJob
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
