using VetSuccess.Domain.Entities;

namespace VetSuccess.Domain.Interfaces;

public interface IPracticeRepository : IRepository<Practice>
{
    Task<Practice?> GetPracticeWithSettingsAsync(string oduId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Practice>> GetActivePracticesAsync(CancellationToken cancellationToken = default);
}
