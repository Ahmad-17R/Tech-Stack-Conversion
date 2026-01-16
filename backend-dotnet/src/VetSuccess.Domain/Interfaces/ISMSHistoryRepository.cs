using VetSuccess.Domain.Entities;

namespace VetSuccess.Domain.Interfaces;

public interface ISMSHistoryRepository : IRepository<SMSHistory>
{
    Task<(List<SMSHistory> smsHistories, int totalCount)> GetContactedClientsAsync(
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);
}
