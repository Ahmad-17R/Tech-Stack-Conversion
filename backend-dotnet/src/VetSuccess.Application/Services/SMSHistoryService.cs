using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VetSuccess.Application.DTOs.SMSHistory;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Shared.Exceptions;

namespace VetSuccess.Application.Services;

public class SMSHistoryService : ISMSHistoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SMSHistoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ContactedClientsResponse> GetContactedClientsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (smsHistories, totalCount) = await _unitOfWork.SMSHistories.GetContactedClientsAsync(page, pageSize, cancellationToken);

        var results = _mapper.Map<List<ContactedClientDto>>(smsHistories);

        return new ContactedClientsResponse
        {
            Count = totalCount,
            Results = results,
            Next = (page * pageSize < totalCount) ? $"?page={page + 1}&page_size={pageSize}" : null,
            Previous = (page > 1) ? $"?page={page - 1}&page_size={pageSize}" : null
        };
    }

    public async Task UpdateSMSHistoryAsync(Guid uuid, SMSHistoryUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        var smsHistory = await _unitOfWork.Repository<Domain.Entities.SMSHistory>()
            .Query()
            .FirstOrDefaultAsync(s => s.Uuid == uuid, cancellationToken);

        if (smsHistory == null)
        {
            throw new NotFoundException("SMSHistory", uuid);
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Status))
        {
            smsHistory.Status = updateDto.Status;
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Notes))
        {
            smsHistory.Notes = updateDto.Notes;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
