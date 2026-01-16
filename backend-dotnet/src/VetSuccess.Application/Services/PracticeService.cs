using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VetSuccess.Application.DTOs.Practice;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Shared.Exceptions;

namespace VetSuccess.Application.Services;

public class PracticeService : IPracticeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PracticeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PracticeDto>> GetPracticesAsync(CancellationToken cancellationToken = default)
    {
        var practices = await _unitOfWork.Practices.GetActivePracticesAsync(cancellationToken);
        return _mapper.Map<List<PracticeDto>>(practices);
    }

    public async Task<List<FAQDto>> GetFAQsAsync(string practiceOduId, CancellationToken cancellationToken = default)
    {
        var practice = await _unitOfWork.Repository<Domain.Entities.Practice>()
            .Query()
            .FirstOrDefaultAsync(p => p.PracticeOduId == practiceOduId, cancellationToken);

        if (practice == null)
        {
            throw new NotFoundException("Practice", practiceOduId);
        }

        var answers = await _unitOfWork.Repository<Domain.Entities.Answer>()
            .Query()
            .Include(a => a.Question)
            .Where(a => a.PracticeOduId == practiceOduId)
            .OrderBy(a => a.Question!.DisplayOrder)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<FAQDto>>(answers);
    }
}
