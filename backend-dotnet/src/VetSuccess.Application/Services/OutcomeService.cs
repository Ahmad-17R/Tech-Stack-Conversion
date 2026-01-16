using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VetSuccess.Application.DTOs.Outcome;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Interfaces;

namespace VetSuccess.Application.Services;

public class OutcomeService : IOutcomeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private const string OutcomesCacheKey = "outcomes:all";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);

    public OutcomeService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<List<OutcomeDto>> GetOutcomesAsync(CancellationToken cancellationToken = default)
    {
        return await _cacheService.GetOrSetAsync(
            OutcomesCacheKey,
            async () =>
            {
                var outcomes = await _unitOfWork.Repository<Domain.Entities.Outcome>()
                    .Query()
                    .OrderBy(o => o.OutcomeName)
                    .ToListAsync(cancellationToken);

                return _mapper.Map<List<OutcomeDto>>(outcomes);
            },
            CacheExpiration,
            cancellationToken);
    }
}
