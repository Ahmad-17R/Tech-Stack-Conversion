using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Shared.Exceptions;

namespace VetSuccess.Application.Services;

public class OutcomeAdminService : IOutcomeAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OutcomeAdminService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<OutcomeAdminDto>> GetAllOutcomesAsync(string? search = null, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<Outcome>().Query();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(o => 
                o.OutcomeName.ToLower().Contains(search) || 
                (o.Description != null && o.Description.ToLower().Contains(search)));
        }

        var outcomes = await query.ToListAsync(cancellationToken);
        return _mapper.Map<List<OutcomeAdminDto>>(outcomes);
    }

    public async Task<OutcomeAdminDto> GetOutcomeByIdAsync(Guid outcomeId, CancellationToken cancellationToken = default)
    {
        var outcome = await _unitOfWork.Repository<Outcome>()
            .Query()
            .FirstOrDefaultAsync(o => o.Uuid == outcomeId, cancellationToken);

        if (outcome == null)
        {
            throw new NotFoundException($"Outcome with ID {outcomeId} not found");
        }

        return _mapper.Map<OutcomeAdminDto>(outcome);
    }

    public async Task<OutcomeAdminDto> CreateOutcomeAsync(CreateOutcomeRequest request, CancellationToken cancellationToken = default)
    {
        // Check if outcome with same ODU ID already exists
        var existing = await _unitOfWork.Repository<Outcome>()
            .Query()
            .FirstOrDefaultAsync(o => o.OutcomeOduId == request.OutcomeOduId, cancellationToken);

        if (existing != null)
        {
            throw new ValidationException($"Outcome with ODU ID '{request.OutcomeOduId}' already exists");
        }

        var outcome = new Outcome
        {
            Uuid = Guid.NewGuid(),
            OutcomeOduId = request.OutcomeOduId,
            OutcomeName = request.OutcomeName,
            Description = request.Description,
            RequiresFollowUp = request.RequiresFollowUp,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Outcome>().AddAsync(outcome, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<OutcomeAdminDto>(outcome);
    }

    public async Task<OutcomeAdminDto> UpdateOutcomeAsync(Guid outcomeId, UpdateOutcomeRequest request, CancellationToken cancellationToken = default)
    {
        var outcome = await _unitOfWork.Repository<Outcome>()
            .Query()
            .FirstOrDefaultAsync(o => o.Uuid == outcomeId, cancellationToken);

        if (outcome == null)
        {
            throw new NotFoundException($"Outcome with ID {outcomeId} not found");
        }

        if (!string.IsNullOrWhiteSpace(request.OutcomeName))
        {
            outcome.OutcomeName = request.OutcomeName;
        }

        if (request.Description != null)
        {
            outcome.Description = request.Description;
        }

        if (request.RequiresFollowUp.HasValue)
        {
            outcome.RequiresFollowUp = request.RequiresFollowUp.Value;
        }

        outcome.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<Outcome>().UpdateAsync(outcome, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<OutcomeAdminDto>(outcome);
    }

    public async Task DeleteOutcomeAsync(Guid outcomeId, CancellationToken cancellationToken = default)
    {
        var outcome = await _unitOfWork.Repository<Outcome>()
            .Query()
            .FirstOrDefaultAsync(o => o.Uuid == outcomeId, cancellationToken);

        if (outcome == null)
        {
            throw new NotFoundException($"Outcome with ID {outcomeId} not found");
        }

        await _unitOfWork.Repository<Outcome>().DeleteAsync(outcome, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
