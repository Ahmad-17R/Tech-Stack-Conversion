using VetSuccess.Domain.Entities;

namespace VetSuccess.Application.Interfaces;

public interface IOutcomeSideEffectsService
{
    Task ProcessOutcomeChangesAsync(IEnumerable<Patient> patients, CancellationToken cancellationToken = default);
}
