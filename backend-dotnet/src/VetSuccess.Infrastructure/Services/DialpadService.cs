using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VetSuccess.Application.Interfaces;
using VetSuccess.Infrastructure.Configuration;

namespace VetSuccess.Infrastructure.Services;

public class DialpadService : IDialpadService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DialpadService> _logger;
    private readonly DialpadOptions _options;

    public DialpadService(
        HttpClient httpClient,
        IOptions<DialpadOptions> options,
        ILogger<DialpadService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ApiToken}");
        _httpClient.Timeout = TimeSpan.FromMilliseconds(_options.TimeoutMilliseconds);
    }

    public async Task<SendSmsResult> SendSmsAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default)
    {
        if (!_options.SendSms)
        {
            _logger.LogInformation("SMS sending is disabled, skipping SMS to {PhoneNumber}", phoneNumber);
            return SendSmsResult.Skipped();
        }

        try
        {
            var request = new
            {
                to = phoneNumber,
                text = message
            };

            var response = await _httpClient.PostAsJsonAsync("sms", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("SMS sent successfully to {PhoneNumber}", phoneNumber);
                return SendSmsResult.Success();
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Failed to send SMS to {PhoneNumber}. Status: {StatusCode}, Error: {Error}",
                phoneNumber, response.StatusCode, errorContent);

            return SendSmsResult.Failed($"HTTP {response.StatusCode}: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception sending SMS to {PhoneNumber}", phoneNumber);
            return SendSmsResult.Failed(ex.Message);
        }
    }
}
