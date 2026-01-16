namespace VetSuccess.Domain.Entities;

public class SMSTemplate : BaseEntity
{
    public string Keywords { get; set; } = string.Empty; // Comma-separated keywords
    public string Template { get; set; } = string.Empty;
    
    public string[] GetKeywords()
    {
        return Keywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
