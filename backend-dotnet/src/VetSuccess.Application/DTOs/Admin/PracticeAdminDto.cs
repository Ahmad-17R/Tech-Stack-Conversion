namespace VetSuccess.Application.DTOs.Admin;

public class PracticeAdminListDto
{
    public string PracticeOduId { get; set; } = null!;
    public string? PracticeName { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Phone { get; set; }
    public string? ServerOduId { get; set; }
    public string? ServerName { get; set; }
    public bool IsArchived { get; set; }
    public bool HasSettings { get; set; }
}

public class PracticeAdminDetailDto
{
    public string PracticeOduId { get; set; } = null!;
    public string? PracticeName { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? Phone { get; set; }
    public string? ServerOduId { get; set; }
    public string? ServerName { get; set; }
    public DateTime? PracticeUpdatedAt { get; set; }
    public DateTime? LatestExtractorUpdated { get; set; }
    public DateTime? LatestTransaction { get; set; }
    public bool IsArchived { get; set; }
    public PracticeSettingsAdminDto? Settings { get; set; }
    public List<AnswerDto>? Answers { get; set; }
}

public class UpdatePracticeAdminRequest
{
    public bool? IsArchived { get; set; }
}
