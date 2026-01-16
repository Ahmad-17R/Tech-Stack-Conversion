namespace VetSuccess.Application.DTOs.Client;

public class ClientListDto
{
    public string ClientOduId { get; set; } = string.Empty;
    public Guid Uuid { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PracticeOduId { get; set; }
    public string? PracticeName { get; set; }
    public DateTime? LastContactedAt { get; set; }
    public string? LastOutcome { get; set; }
    public int PatientCount { get; set; }
}
