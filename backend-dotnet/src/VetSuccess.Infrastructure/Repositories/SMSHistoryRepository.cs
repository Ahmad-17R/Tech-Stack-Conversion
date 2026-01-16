using Microsoft.EntityFrameworkCore;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Infrastructure.Data;

namespace VetSuccess.Infrastructure.Repositories;

public class SMSHistoryRepository : GenericRepository<SMSHistory>, ISMSHistoryRepository
{
    public SMSHistoryRepository(VetSuccessDbContext context) : base(context)
    {
    }

    public async Task<(List<SMSHistory> smsHistories, int totalCount)> GetContactedClientsAsync(
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(s => s.Status == "SENT")
            .Where(s => s.Practice != null && !s.Practice.IsArchived)
            .Include(s => s.Client)
            .Include(s => s.Practice)
            .Include(s => s.Reminders)
                .ThenInclude(r => r.Patient)
            .OrderByDescending(s => s.SentAt);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var offset = (page - 1) * pageSize;
        var smsHistories = await query
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (smsHistories, totalCount);
    }
}
