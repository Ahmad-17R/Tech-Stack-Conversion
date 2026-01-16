namespace VetSuccess.Domain.Entities;

public class UpdatesEmailEvent : BaseEntity
{
    public string Status { get; set; } = "PENDING"; // NO_FILES, PENDING, SENT, ERROR
    public List<string> FilePaths { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public string PracticeId { get; set; } = string.Empty;
    
    // Navigation property
    public Practice? Practice { get; set; }
}
