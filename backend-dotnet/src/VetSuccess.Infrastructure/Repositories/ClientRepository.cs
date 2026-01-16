using Microsoft.EntityFrameworkCore;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Infrastructure.Data;

namespace VetSuccess.Infrastructure.Repositories;

public class ClientRepository : GenericRepository<Client>, IClientRepository
{
    public ClientRepository(VetSuccessDbContext context) : base(context)
    {
    }

    public async Task<Client?> GetClientWithDetailsAsync(string oduId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.ClientOduId == oduId)
            .Where(c => c.PimsIsDeleted != true)
            .Where(c => c.PimsIsInactive != true)
            .Where(c => c.IsHomePractice != false)
            .Include(c => c.Practice)
            .Include(c => c.Server)
            .Include(c => c.Emails.Where(e => e.IsPreferred == true && e.ExtractorRemovedAt == null))
            .Include(c => c.Phones.Where(p => p.IsPreferred == true && p.ExtractorRemovedAt == null))
            .Include(c => c.Addresses.Where(a => a.ExtractorRemovedAt == null))
            .Include(c => c.ClientPatientRelationships)
                .ThenInclude(r => r.Patient)
                    .ThenInclude(p => p!.Outcome)
            .Include(c => c.ClientPatientRelationships)
                .ThenInclude(r => r.Patient)
                    .ThenInclude(p => p!.Appointments.Where(a => 
                        a.IsCanceledAppointment != true &&
                        a.AppointmentDate != null)
                        .OrderByDescending(a => a.AppointmentDate))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Client>> SearchClientsAsync(string? searchTerm, string? practiceOduId, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(c => c.PimsIsDeleted != true)
            .Where(c => c.PimsIsInactive != true)
            .Where(c => c.IsHomePractice != false)
            .Include(c => c.Practice)
            .Include(c => c.Emails.Where(e => e.IsPreferred == true && e.ExtractorRemovedAt == null))
            .Include(c => c.Phones.Where(p => p.IsPreferred == true && p.ExtractorRemovedAt == null))
            .Include(c => c.ClientPatientRelationships)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(practiceOduId))
        {
            query = query.Where(c => c.PracticeOduId == practiceOduId);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            
            // Search by client name, ODU ID, or email
            query = query.Where(c => 
                (c.FullName != null && c.FullName.ToLower().Contains(searchLower)) ||
                c.ClientOduId.ToLower().Contains(searchLower) ||
                c.Emails.Any(e => e.EmailAddress != null && e.EmailAddress.ToLower().Contains(searchLower) && e.ExtractorRemovedAt == null));
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Client>> SearchClientsByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.PimsIsDeleted != true)
            .Where(c => c.PimsIsInactive != true)
            .Where(c => c.IsHomePractice != false)
            .Where(c => c.Phones.Any(p => p.AppNumber == phoneNumber && p.ExtractorRemovedAt == null))
            .Include(c => c.Practice)
            .Include(c => c.Emails.Where(e => e.IsPreferred == true && e.ExtractorRemovedAt == null))
            .Include(c => c.Phones.Where(p => p.IsPreferred == true && p.ExtractorRemovedAt == null))
            .ToListAsync(cancellationToken);
    }
}
