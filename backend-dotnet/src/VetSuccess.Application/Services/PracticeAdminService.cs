using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Shared.Exceptions;

namespace VetSuccess.Application.Services;

public class PracticeAdminService : IPracticeAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PracticeAdminService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PracticeAdminListDto>> GetAllPracticesAsync(
        bool? hasSettings = null,
        bool? isSmsMailingEnabled = null,
        bool? isEmailUpdatesEnabled = null,
        bool? isArchived = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Practice> query = _unitOfWork.Repository<Practice>()
            .Query()
            .Include(p => p.Server)
            .Include(p => p.PracticeSettings);

        if (hasSettings.HasValue)
        {
            if (hasSettings.Value)
            {
                query = query.Where(p => p.PracticeSettings != null);
            }
            else
            {
                query = query.Where(p => p.PracticeSettings == null);
            }
        }

        if (isSmsMailingEnabled.HasValue)
        {
            query = query.Where(p => p.PracticeSettings != null && p.PracticeSettings.IsSmsMailingEnabled == isSmsMailingEnabled.Value);
        }

        if (isEmailUpdatesEnabled.HasValue)
        {
            query = query.Where(p => p.PracticeSettings != null && p.PracticeSettings.IsEmailUpdatesEnabled == isEmailUpdatesEnabled.Value);
        }

        if (isArchived.HasValue)
        {
            query = query.Where(p => p.IsArchived == isArchived.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(p =>
                p.PracticeName.ToLower().Contains(search) ||
                (p.City != null && p.City.ToLower().Contains(search)) ||
                (p.State != null && p.State.ToLower().Contains(search)) ||
                (p.ZipCode != null && p.ZipCode.Contains(search)) ||
                (p.Phone != null && p.Phone.Contains(search)));
        }

        query = query.OrderBy(p => p.PracticeName);

        var practices = await query.ToListAsync(cancellationToken);
        return _mapper.Map<List<PracticeAdminListDto>>(practices);
    }

    public async Task<PracticeAdminDetailDto> GetPracticeByIdAsync(string practiceOduId, CancellationToken cancellationToken = default)
    {
        var practice = await _unitOfWork.Repository<Practice>()
            .Query()
            .Include(p => p.Server)
            .Include(p => p.PracticeSettings)
            .Include(p => p.Answers)
                .ThenInclude(a => a.Question)
            .FirstOrDefaultAsync(p => p.PracticeOduId == practiceOduId, cancellationToken);

        if (practice == null)
        {
            throw new NotFoundException($"Practice with ODU ID '{practiceOduId}' not found");
        }

        return _mapper.Map<PracticeAdminDetailDto>(practice);
    }

    public async Task<PracticeAdminDetailDto> UpdatePracticeAsync(string practiceOduId, UpdatePracticeAdminRequest request, CancellationToken cancellationToken = default)
    {
        var practice = await _unitOfWork.Repository<Practice>()
            .Query()
            .Include(p => p.Server)
            .Include(p => p.PracticeSettings)
            .Include(p => p.Answers)
                .ThenInclude(a => a.Question)
            .FirstOrDefaultAsync(p => p.PracticeOduId == practiceOduId, cancellationToken);

        if (practice == null)
        {
            throw new NotFoundException($"Practice with ODU ID '{practiceOduId}' not found");
        }

        if (request.IsArchived.HasValue)
        {
            practice.IsArchived = request.IsArchived.Value;
        }

        await _unitOfWork.Repository<Practice>().UpdateAsync(practice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PracticeAdminDetailDto>(practice);
    }
}
