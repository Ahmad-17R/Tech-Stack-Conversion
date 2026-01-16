using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Shared.Exceptions;

namespace VetSuccess.Application.Services;

public class AnswerAdminService : IAnswerAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AnswerAdminService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<AnswerDto>> GetAllAnswersAsync(string? practiceOduId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Answer> query = _unitOfWork.Repository<Answer>()
            .Query()
            .Include(a => a.Practice)
            .Include(a => a.Question);

        if (!string.IsNullOrWhiteSpace(practiceOduId))
        {
            query = query.Where(a => a.PracticeOduId == practiceOduId);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(a => 
                a.AnswerText.ToLower().Contains(search) ||
                a.Practice.PracticeName.ToLower().Contains(search) ||
                a.Question.QuestionText.ToLower().Contains(search));
        }

        var answers = await query.ToListAsync(cancellationToken);
        return _mapper.Map<List<AnswerDto>>(answers);
    }

    public async Task<AnswerDto> GetAnswerByIdAsync(Guid answerId, CancellationToken cancellationToken = default)
    {
        var answer = await _unitOfWork.Repository<Answer>()
            .Query()
            .Include(a => a.Practice)
            .Include(a => a.Question)
            .FirstOrDefaultAsync(a => a.Uuid == answerId, cancellationToken);

        if (answer == null)
        {
            throw new NotFoundException($"Answer with ID {answerId} not found");
        }

        return _mapper.Map<AnswerDto>(answer);
    }

    public async Task<AnswerDto> CreateAnswerAsync(CreateAnswerRequest request, CancellationToken cancellationToken = default)
    {
        // Validate practice exists
        var practice = await _unitOfWork.Repository<Practice>()
            .Query()
            .FirstOrDefaultAsync(p => p.PracticeOduId == request.PracticeOduId, cancellationToken);

        if (practice == null)
        {
            throw new NotFoundException($"Practice with ODU ID '{request.PracticeOduId}' not found");
        }

        // Validate question exists
        var question = await _unitOfWork.Repository<Question>()
            .Query()
            .FirstOrDefaultAsync(q => q.Uuid == request.QuestionId, cancellationToken);

        if (question == null)
        {
            throw new NotFoundException($"Question with ID {request.QuestionId} not found");
        }

        // Check for duplicate answer (unique constraint per practice-question pair)
        var existing = await _unitOfWork.Repository<Answer>()
            .Query()
            .FirstOrDefaultAsync(a => a.PracticeOduId == request.PracticeOduId && a.QuestionId == request.QuestionId, cancellationToken);

        if (existing != null)
        {
            throw new ValidationException($"An answer already exists for this practice and question combination");
        }

        var answer = new Answer
        {
            Uuid = Guid.NewGuid(),
            PracticeOduId = request.PracticeOduId,
            QuestionId = request.QuestionId,
            AnswerText = request.AnswerText,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Answer>().AddAsync(answer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with includes
        return await GetAnswerByIdAsync(answer.Uuid, cancellationToken);
    }

    public async Task<AnswerDto> UpdateAnswerAsync(Guid answerId, UpdateAnswerRequest request, CancellationToken cancellationToken = default)
    {
        var answer = await _unitOfWork.Repository<Answer>()
            .Query()
            .Include(a => a.Practice)
            .Include(a => a.Question)
            .FirstOrDefaultAsync(a => a.Uuid == answerId, cancellationToken);

        if (answer == null)
        {
            throw new NotFoundException($"Answer with ID {answerId} not found");
        }

        if (!string.IsNullOrWhiteSpace(request.AnswerText))
        {
            answer.AnswerText = request.AnswerText;
        }

        if (request.QuestionId.HasValue && request.QuestionId.Value != answer.QuestionId)
        {
            // Validate new question exists
            var question = await _unitOfWork.Repository<Question>()
                .Query()
                .FirstOrDefaultAsync(q => q.Uuid == request.QuestionId.Value, cancellationToken);

            if (question == null)
            {
                throw new NotFoundException($"Question with ID {request.QuestionId.Value} not found");
            }

            // Check for duplicate
            var existing = await _unitOfWork.Repository<Answer>()
                .Query()
                .FirstOrDefaultAsync(a => a.PracticeOduId == answer.PracticeOduId && a.QuestionId == request.QuestionId.Value && a.Uuid != answerId, cancellationToken);

            if (existing != null)
            {
                throw new ValidationException($"An answer already exists for this practice and question combination");
            }

            answer.QuestionId = request.QuestionId.Value;
        }

        answer.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<Answer>().UpdateAsync(answer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AnswerDto>(answer);
    }

    public async Task DeleteAnswerAsync(Guid answerId, CancellationToken cancellationToken = default)
    {
        var answer = await _unitOfWork.Repository<Answer>()
            .Query()
            .FirstOrDefaultAsync(a => a.Uuid == answerId, cancellationToken);

        if (answer == null)
        {
            throw new NotFoundException($"Answer with ID {answerId} not found");
        }

        await _unitOfWork.Repository<Answer>().DeleteAsync(answer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
