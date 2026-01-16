using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Shared.Exceptions;

namespace VetSuccess.Application.Services;

public class PracticeSettingsAdminService : IPracticeSettingsAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private const int MIN_EXPIRY_PERIOD_IN_WEEKS = 4;

    public PracticeSettingsAdminService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PracticeSettingsAdminDto>> GetAllPracticeSettingsAsync(string? practiceOduId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<PracticeSettings> query = _unitOfWork.Repository<PracticeSettings>()
            .Query()
            .Include(s => s.Practice);

        if (!string.IsNullOrWhiteSpace(practiceOduId))
        {
            query = query.Where(s => s.PracticeOduId == practiceOduId);
        }

        var settings = await query.ToListAsync(cancellationToken);
        return _mapper.Map<List<PracticeSettingsAdminDto>>(settings);
    }

    public async Task<PracticeSettingsAdminDto> GetPracticeSettingsByIdAsync(Guid settingsId, CancellationToken cancellationToken = default)
    {
        var settings = await _unitOfWork.Repository<PracticeSettings>()
            .Query()
            .Include(s => s.Practice)
            .FirstOrDefaultAsync(s => s.Uuid == settingsId, cancellationToken);

        if (settings == null)
        {
            throw new NotFoundException($"Practice settings with ID {settingsId} not found");
        }

        return _mapper.Map<PracticeSettingsAdminDto>(settings);
    }

    public async Task<PracticeSettingsAdminDto> CreatePracticeSettingsAsync(CreatePracticeSettingsRequest request, CancellationToken cancellationToken = default)
    {
        // Validate practice exists
        var practice = await _unitOfWork.Repository<Practice>()
            .Query()
            .FirstOrDefaultAsync(p => p.PracticeOduId == request.PracticeOduId, cancellationToken);

        if (practice == null)
        {
            throw new NotFoundException($"Practice with ODU ID '{request.PracticeOduId}' not found");
        }

        // Check if settings already exist for this practice
        var existing = await _unitOfWork.Repository<PracticeSettings>()
            .Query()
            .FirstOrDefaultAsync(s => s.PracticeOduId == request.PracticeOduId, cancellationToken);

        if (existing != null)
        {
            throw new ValidationException($"Practice settings already exist for practice '{request.PracticeOduId}'");
        }

        // Validate dates
        ValidateDates(request.LaunchDate, request.StartDateForLaunch, request.EndDateForLaunch);

        var settings = new PracticeSettings
        {
            Uuid = Guid.NewGuid(),
            PracticeOduId = request.PracticeOduId,
            IsSmsMailingEnabled = request.IsSmsMailingEnabled,
            SmsSendersPhone = request.SmsSendersPhone,
            SmsPracticeName = request.SmsPracticeName,
            SmsScheduler = request.SmsScheduler,
            SmsLink = request.SmsLink,
            SmsPhone = request.SmsPhone,
            IsEmailUpdatesEnabled = request.IsEmailUpdatesEnabled,
            Email = request.Email,
            SchedulerEmail = request.SchedulerEmail,
            RdoName = request.RdoName,
            RdoEmail = request.RdoEmail,
            LaunchDate = request.LaunchDate,
            StartDateForLaunch = request.StartDateForLaunch,
            EndDateForLaunch = request.EndDateForLaunch,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<PracticeSettings>().AddAsync(settings, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetPracticeSettingsByIdAsync(settings.Uuid, cancellationToken);
    }

    public async Task<PracticeSettingsAdminDto> UpdatePracticeSettingsAsync(Guid settingsId, UpdatePracticeSettingsRequest request, CancellationToken cancellationToken = default)
    {
        var settings = await _unitOfWork.Repository<PracticeSettings>()
            .Query()
            .Include(s => s.Practice)
            .FirstOrDefaultAsync(s => s.Uuid == settingsId, cancellationToken);

        if (settings == null)
        {
            throw new NotFoundException($"Practice settings with ID {settingsId} not found");
        }

        // Validate dates if any are being updated
        var launchDate = request.LaunchDate ?? settings.LaunchDate;
        var startDate = request.StartDateForLaunch ?? settings.StartDateForLaunch;
        var endDate = request.EndDateForLaunch ?? settings.EndDateForLaunch;
        ValidateDates(launchDate, startDate, endDate);

        if (request.IsSmsMailingEnabled.HasValue)
            settings.IsSmsMailingEnabled = request.IsSmsMailingEnabled.Value;

        if (request.SmsSendersPhone != null)
            settings.SmsSendersPhone = request.SmsSendersPhone;

        if (request.SmsPracticeName != null)
            settings.SmsPracticeName = request.SmsPracticeName;

        if (request.SmsScheduler != null)
            settings.SmsScheduler = request.SmsScheduler;

        if (request.SmsLink != null)
            settings.SmsLink = request.SmsLink;

        if (request.SmsPhone != null)
            settings.SmsPhone = request.SmsPhone;

        if (request.IsEmailUpdatesEnabled.HasValue)
            settings.IsEmailUpdatesEnabled = request.IsEmailUpdatesEnabled.Value;

        if (request.Email != null)
            settings.Email = request.Email;

        if (request.SchedulerEmail != null)
            settings.SchedulerEmail = request.SchedulerEmail;

        if (request.RdoName != null)
            settings.RdoName = request.RdoName;

        if (request.RdoEmail != null)
            settings.RdoEmail = request.RdoEmail;

        if (request.LaunchDate.HasValue)
            settings.LaunchDate = request.LaunchDate;

        if (request.StartDateForLaunch.HasValue)
            settings.StartDateForLaunch = request.StartDateForLaunch;

        if (request.EndDateForLaunch.HasValue)
            settings.EndDateForLaunch = request.EndDateForLaunch;

        settings.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<PracticeSettings>().UpdateAsync(settings, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PracticeSettingsAdminDto>(settings);
    }

    public async Task DeletePracticeSettingsAsync(Guid settingsId, CancellationToken cancellationToken = default)
    {
        var settings = await _unitOfWork.Repository<PracticeSettings>()
            .Query()
            .FirstOrDefaultAsync(s => s.Uuid == settingsId, cancellationToken);

        if (settings == null)
        {
            throw new NotFoundException($"Practice settings with ID {settingsId} not found");
        }

        await _unitOfWork.Repository<PracticeSettings>().DeleteAsync(settings, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private void ValidateDates(DateTime? launchDate, DateTime? startDate, DateTime? endDate)
    {
        // End date requires start date
        if (endDate.HasValue && !startDate.HasValue)
        {
            throw new ValidationException("End date for launch requires a start date for launch");
        }

        // End date must be after start date
        if (endDate.HasValue && startDate.HasValue && endDate.Value <= startDate.Value)
        {
            throw new ValidationException("End date for launch must be after start date for launch");
        }

        // Start date must be at least MIN_EXPIRY_PERIOD_IN_WEEKS before launch date
        if (startDate.HasValue && launchDate.HasValue)
        {
            var minStartDate = launchDate.Value.AddDays(-MIN_EXPIRY_PERIOD_IN_WEEKS * 7);
            if (startDate.Value >= minStartDate)
            {
                throw new ValidationException($"Start date for launch must be at least {MIN_EXPIRY_PERIOD_IN_WEEKS} weeks before launch date");
            }
        }
    }
}
