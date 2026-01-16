using VetSuccess.Application.DTOs.Practice;

namespace VetSuccess.Application.Interfaces;

public interface IPracticeService
{
    Task<List<PracticeDto>> GetPracticesAsync(CancellationToken cancellationToken = default);
    Task<List<FAQDto>> GetFAQsAsync(string practiceOduId, CancellationToken cancellationToken = default);
}
