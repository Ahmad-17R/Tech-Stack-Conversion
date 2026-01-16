using VetSuccess.Application.DTOs.Admin;

namespace VetSuccess.Application.Interfaces;

public interface IUserAdminService
{
    Task<List<UserListDto>> GetAllUsersAsync(bool? isSuperuser = null, bool? isActive = null, string? search = null, CancellationToken cancellationToken = default);
    Task<UserDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IOutcomeAdminService
{
    Task<List<OutcomeAdminDto>> GetAllOutcomesAsync(string? search = null, CancellationToken cancellationToken = default);
    Task<OutcomeAdminDto> GetOutcomeByIdAsync(Guid outcomeId, CancellationToken cancellationToken = default);
    Task<OutcomeAdminDto> CreateOutcomeAsync(CreateOutcomeRequest request, CancellationToken cancellationToken = default);
    Task<OutcomeAdminDto> UpdateOutcomeAsync(Guid outcomeId, UpdateOutcomeRequest request, CancellationToken cancellationToken = default);
    Task DeleteOutcomeAsync(Guid outcomeId, CancellationToken cancellationToken = default);
}

public interface IQuestionAdminService
{
    Task<List<QuestionDto>> GetAllQuestionsAsync(string? search = null, CancellationToken cancellationToken = default);
    Task<QuestionDto> GetQuestionByIdAsync(Guid questionId, CancellationToken cancellationToken = default);
    Task<QuestionDto> CreateQuestionAsync(CreateQuestionRequest request, CancellationToken cancellationToken = default);
    Task<QuestionDto> UpdateQuestionAsync(Guid questionId, UpdateQuestionRequest request, CancellationToken cancellationToken = default);
    Task DeleteQuestionAsync(Guid questionId, CancellationToken cancellationToken = default);
}

public interface IAnswerAdminService
{
    Task<List<AnswerDto>> GetAllAnswersAsync(string? practiceOduId = null, string? search = null, CancellationToken cancellationToken = default);
    Task<AnswerDto> GetAnswerByIdAsync(Guid answerId, CancellationToken cancellationToken = default);
    Task<AnswerDto> CreateAnswerAsync(CreateAnswerRequest request, CancellationToken cancellationToken = default);
    Task<AnswerDto> UpdateAnswerAsync(Guid answerId, UpdateAnswerRequest request, CancellationToken cancellationToken = default);
    Task DeleteAnswerAsync(Guid answerId, CancellationToken cancellationToken = default);
}

public interface IPracticeAdminService
{
    Task<List<PracticeAdminListDto>> GetAllPracticesAsync(
        bool? hasSettings = null,
        bool? isSmsMailingEnabled = null,
        bool? isEmailUpdatesEnabled = null,
        bool? isArchived = null,
        string? search = null,
        CancellationToken cancellationToken = default);
    Task<PracticeAdminDetailDto> GetPracticeByIdAsync(string practiceOduId, CancellationToken cancellationToken = default);
    Task<PracticeAdminDetailDto> UpdatePracticeAsync(string practiceOduId, UpdatePracticeAdminRequest request, CancellationToken cancellationToken = default);
}

public interface IPracticeSettingsAdminService
{
    Task<List<PracticeSettingsAdminDto>> GetAllPracticeSettingsAsync(string? practiceOduId = null, CancellationToken cancellationToken = default);
    Task<PracticeSettingsAdminDto> GetPracticeSettingsByIdAsync(Guid settingsId, CancellationToken cancellationToken = default);
    Task<PracticeSettingsAdminDto> CreatePracticeSettingsAsync(CreatePracticeSettingsRequest request, CancellationToken cancellationToken = default);
    Task<PracticeSettingsAdminDto> UpdatePracticeSettingsAsync(Guid settingsId, UpdatePracticeSettingsRequest request, CancellationToken cancellationToken = default);
    Task DeletePracticeSettingsAsync(Guid settingsId, CancellationToken cancellationToken = default);
}

public interface ISMSTemplateAdminService
{
    Task<List<SMSTemplateDto>> GetAllSMSTemplatesAsync(CancellationToken cancellationToken = default);
    Task<SMSTemplateDto> GetSMSTemplateByIdAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<SMSTemplateDto> CreateSMSTemplateAsync(CreateSMSTemplateRequest request, CancellationToken cancellationToken = default);
    Task<SMSTemplateDto> UpdateSMSTemplateAsync(Guid templateId, UpdateSMSTemplateRequest request, CancellationToken cancellationToken = default);
    Task DeleteSMSTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
}
