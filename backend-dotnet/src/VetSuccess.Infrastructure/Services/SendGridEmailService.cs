using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using VetSuccess.Application.Interfaces;
using VetSuccess.Infrastructure.Configuration;
using VetSuccess.Shared.Constants;

namespace VetSuccess.Infrastructure.Services;

public class SendGridEmailService : IEmailService
{
    private readonly ISendGridClient _sendGridClient;
    private readonly EmailOptions _options;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(
        IOptions<EmailOptions> options,
        ILogger<SendGridEmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
        _sendGridClient = new SendGridClient(_options.SendGridApiKey);
    }

    public async Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromEmail = string.IsNullOrEmpty(message.FromEmail) 
                ? _options.DefaultFromEmail 
                : message.FromEmail;
            var fromName = string.IsNullOrEmpty(message.FromName) 
                ? _options.DefaultFromName 
                : message.FromName;

            var from = new EmailAddress(fromEmail, fromName);
            
            // Use debug email if configured
            var toEmail = _options.UseDebugEmail && !string.IsNullOrEmpty(_options.DebugRecipient)
                ? _options.DebugRecipient
                : message.To;
            
            var to = new EmailAddress(toEmail);

            var sendGridMessage = MailHelper.CreateSingleEmail(
                from,
                to,
                message.Subject,
                message.PlainTextContent,
                message.HtmlContent);

            // Add attachments
            foreach (var attachment in message.Attachments)
            {
                var base64Content = Convert.ToBase64String(attachment.Content);
                sendGridMessage.AddAttachment(attachment.Filename, base64Content, attachment.MimeType);
            }

            // Add CC emails
            foreach (var ccEmail in message.CcEmails)
            {
                sendGridMessage.AddCc(new EmailAddress(ccEmail));
            }

            // Add CC if debug mode and CC recipient is configured
            if (_options.UseDebugEmail && !string.IsNullOrEmpty(_options.DebugCcRecipient))
            {
                sendGridMessage.AddCc(new EmailAddress(_options.DebugCcRecipient));
            }

            var response = await _sendGridClient.SendEmailAsync(sendGridMessage, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully to {To}", toEmail);
            }
            else
            {
                var body = await response.Body.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to send email to {To}. Status: {StatusCode}, Error: {Error}",
                    toEmail, response.StatusCode, body);
                throw new Exception($"Failed to send email: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception sending email to {To}", message.To);
            throw;
        }
    }

    public async Task SendDailyUpdatesAsync(
        string toEmail, 
        List<EmailAttachment> attachments, 
        List<string>? ccEmails = null, 
        CancellationToken cancellationToken = default)
    {
        var pluralValue = attachments.Count > 1 ? "s" : "";
        var body = string.Format(EmailConstants.DailyUpdatesBody, pluralValue);

        var message = new EmailMessage
        {
            To = toEmail,
            Subject = EmailConstants.DailyUpdatesSubject,
            PlainTextContent = body,
            HtmlContent = body.Replace("\n", "<br>"),
            Attachments = attachments,
            CcEmails = ccEmails ?? new List<string>()
        };

        await SendEmailAsync(message, cancellationToken);
    }
}
