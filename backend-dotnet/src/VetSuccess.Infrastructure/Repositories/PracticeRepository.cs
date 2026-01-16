using Microsoft.EntityFrameworkCore;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Infrastructure.Data;

namespace VetSuccess.Infrastructure.Repositories;

public class PracticeRepository : GenericRepository<Practice>, IPracticeRepository
{
    public PracticeRepository(VetSuccessDbContext context) : base(context)
    {
    }

    public async Task<Practice?> GetPracticeWithSettingsAsync(string oduId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.PracticeSettings)
            .FirstOrDefaultAsync(p => p.PracticeOduId == oduId, cancellationToken);
    }

    public async Task<IEnumerable<Practice>> GetActivePracticesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => !p.IsArchived)
            .Include(p => p.PracticeSettings)
            .ToListAsync(cancellationToken);
    }
}
