using Microsoft.EntityFrameworkCore;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Infrastructure.Data;

namespace VetSuccess.Infrastructure.Repositories;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(VetSuccessDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Patient>> GetPatientsByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Clients.Any(c => c.ClientOduId == clientId))
            .Where(p => p.PimsIsDeceased != true)
            .Where(p => p.PimsIsInactive != true)
            .Where(p => p.PimsIsDeleted != true)
            .Where(p => p.IsDeceased != true)
            .ToListAsync(cancellationToken);
    }
}
