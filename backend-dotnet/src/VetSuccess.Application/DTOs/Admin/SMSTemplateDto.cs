namespace VetSuccess.Application.DTOs.Admin;

public class SMSTemplateDto
{
    public Guid Id { get; set; }
    public string KeyWords { get; set; } = null!;
    public string Template { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateSMSTemplateRequest
{
    public string KeyWords { get; set; } = null!;
    public string Template { get; set; } = null!;
}

public class UpdateSMSTemplateRequest
{
    public string? KeyWords { get; set; }
    public string? Template { get; set; }
}
