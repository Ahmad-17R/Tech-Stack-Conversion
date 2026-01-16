namespace VetSuccess.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);
    Task SendDailyUpdatesAsync(string toEmail, List<EmailAttachment> attachments, List<string>? ccEmails = null, CancellationToken cancellationToken = default);
}

public class EmailMessage
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? PlainTextContent { get; set; }
    public string? HtmlContent { get; set; }
    public string? FromEmail { get; set; }
    public string? FromName { get; set; }
    public List<EmailAttachment> Attachments { get; set; } = new();
    public List<string> CcEmails { get; set; } = new();
}

public class EmailAttachment
{
    public string Filename { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
}
