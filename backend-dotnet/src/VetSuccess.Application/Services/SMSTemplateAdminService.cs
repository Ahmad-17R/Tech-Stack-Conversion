using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Shared.Exceptions;

namespace VetSuccess.Application.Services;

public class SMSTemplateAdminService : ISMSTemplateAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    // Valid SMS template variables
    private static readonly HashSet<string> ValidVariables = new()
    {
        "practice_name",
        "practice_phone",
        "practice_link",
        "scheduler",
        "client_name",
        "patient_name",
        "appointment_date",
        "reminder_description"
    };

    public SMSTemplateAdminService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<List<SMSTemplateDto>> GetAllSMSTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var templates = await _unitOfWork.Repository<SMSTemplate>()
            .Query()
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<SMSTemplateDto>>(templates);
    }

    public async Task<SMSTemplateDto> GetSMSTemplateByIdAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        var template = await _unitOfWork.Repository<SMSTemplate>()
            .Query()
            .FirstOrDefaultAsync(t => t.Uuid == templateId, cancellationToken);

        if (template == null)
        {
            throw new NotFoundException($"SMS template with ID {templateId} not found");
        }

        return _mapper.Map<SMSTemplateDto>(template);
    }

    public async Task<SMSTemplateDto> CreateSMSTemplateAsync(CreateSMSTemplateRequest request, CancellationToken cancellationToken = default)
    {
        // Validate template variables
        ValidateTemplateVariables(request.Template);

        var template = new SMSTemplate
        {
            Uuid = Guid.NewGuid(),
            Keywords = request.KeyWords,
            Template = request.Template,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<SMSTemplate>().AddAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await InvalidateSMSTemplateCache();

        return _mapper.Map<SMSTemplateDto>(template);
    }

    public async Task<SMSTemplateDto> UpdateSMSTemplateAsync(Guid templateId, UpdateSMSTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var template = await _unitOfWork.Repository<SMSTemplate>()
            .Query()
            .FirstOrDefaultAsync(t => t.Uuid == templateId, cancellationToken);

        if (template == null)
        {
            throw new NotFoundException($"SMS template with ID {templateId} not found");
        }

        if (!string.IsNullOrWhiteSpace(request.KeyWords))
        {
            template.Keywords = request.KeyWords;
        }

        if (!string.IsNullOrWhiteSpace(request.Template))
        {
            ValidateTemplateVariables(request.Template);
            template.Template = request.Template;
        }

        template.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<SMSTemplate>().UpdateAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await InvalidateSMSTemplateCache();

        return _mapper.Map<SMSTemplateDto>(template);
    }

    public async Task DeleteSMSTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        var template = await _unitOfWork.Repository<SMSTemplate>()
            .Query()
            .FirstOrDefaultAsync(t => t.Uuid == templateId, cancellationToken);

        if (template == null)
        {
            throw new NotFoundException($"SMS template with ID {templateId} not found");
        }

        await _unitOfWork.Repository<SMSTemplate>().DeleteAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await InvalidateSMSTemplateCache();
    }

    private void ValidateTemplateVariables(string template)
    {
        if (string.IsNullOrWhiteSpace(template))
        {
            return;
        }

        // Find all variables in the template (format: {variable_name})
        var pattern = @"\{([^}]+)\}";
        var matches = Regex.Matches(template, pattern);

        var invalidVariables = new List<string>();
        foreach (Match match in matches)
        {
            var variable = match.Groups[1].Value;
            if (!ValidVariables.Contains(variable))
            {
                invalidVariables.Add(variable);
            }
        }

        if (invalidVariables.Any())
        {
            throw new ValidationException($"Invalid template variables: {string.Join(", ", invalidVariables)}. Valid variables are: {string.Join(", ", ValidVariables)}");
        }
    }

    private async Task InvalidateSMSTemplateCache()
    {
        // Invalidate any SMS template related cache keys
        await _cacheService.RemoveAsync("sms_templates");
    }
}
