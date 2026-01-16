using VetSuccess.Application.DTOs.SMSHistory;

namespace VetSuccess.Application.Interfaces;

public interface ISMSHistoryService
{
    Task<ContactedClientsResponse> GetContactedClientsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task UpdateSMSHistoryAsync(Guid uuid, SMSHistoryUpdateDto updateDto, CancellationToken cancellationToken = default);
}
