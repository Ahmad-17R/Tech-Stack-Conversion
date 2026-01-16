namespace VetSuccess.Application.Interfaces;

public interface ISmsAggregationJob
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
