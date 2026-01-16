using VetSuccess.Application.DTOs.Outcome;

namespace VetSuccess.Application.Interfaces;

public interface IOutcomeService
{
    Task<List<OutcomeDto>> GetOutcomesAsync(CancellationToken cancellationToken = default);
}
