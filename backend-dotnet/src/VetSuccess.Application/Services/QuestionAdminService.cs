using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Shared.Exceptions;

namespace VetSuccess.Application.Services;

public class QuestionAdminService : IQuestionAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public QuestionAdminService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<QuestionDto>> GetAllQuestionsAsync(string? search = null, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<Question>().Query();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(q => q.QuestionText.ToLower().Contains(search));
        }

        query = query.OrderBy(q => q.DisplayOrder);

        var questions = await query.ToListAsync(cancellationToken);
        return _mapper.Map<List<QuestionDto>>(questions);
    }

    public async Task<QuestionDto> GetQuestionByIdAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        var question = await _unitOfWork.Repository<Question>()
            .Query()
            .FirstOrDefaultAsync(q => q.Uuid == questionId, cancellationToken);

        if (question == null)
        {
            throw new NotFoundException($"Question with ID {questionId} not found");
        }

        return _mapper.Map<QuestionDto>(question);
    }

    public async Task<QuestionDto> CreateQuestionAsync(CreateQuestionRequest request, CancellationToken cancellationToken = default)
    {
        var question = new Question
        {
            Uuid = Guid.NewGuid(),
            QuestionText = request.QuestionText,
            DisplayOrder = request.DisplayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Question>().AddAsync(question, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<QuestionDto>(question);
    }

    public async Task<QuestionDto> UpdateQuestionAsync(Guid questionId, UpdateQuestionRequest request, CancellationToken cancellationToken = default)
    {
        var question = await _unitOfWork.Repository<Question>()
            .Query()
            .FirstOrDefaultAsync(q => q.Uuid == questionId, cancellationToken);

        if (question == null)
        {
            throw new NotFoundException($"Question with ID {questionId} not found");
        }

        if (!string.IsNullOrWhiteSpace(request.QuestionText))
        {
            question.QuestionText = request.QuestionText;
        }

        if (request.DisplayOrder.HasValue)
        {
            question.DisplayOrder = request.DisplayOrder.Value;
        }

        question.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<Question>().UpdateAsync(question, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<QuestionDto>(question);
    }

    public async Task DeleteQuestionAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        var question = await _unitOfWork.Repository<Question>()
            .Query()
            .FirstOrDefaultAsync(q => q.Uuid == questionId, cancellationToken);

        if (question == null)
        {
            throw new NotFoundException($"Question with ID {questionId} not found");
        }

        await _unitOfWork.Repository<Question>().DeleteAsync(question, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
