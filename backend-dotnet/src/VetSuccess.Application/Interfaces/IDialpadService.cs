namespace VetSuccess.Application.Interfaces;

public interface IDialpadService
{
    Task<SendSmsResult> SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}

public class SendSmsResult
{
    public bool IsSuccess { get; set; }
    public bool IsSkipped { get; set; }
    public string? ErrorMessage { get; set; }

    public static SendSmsResult Success() => new() { IsSuccess = true };
    public static SendSmsResult Skipped() => new() { IsSkipped = true };
    public static SendSmsResult Failed(string errorMessage) => new() { IsSuccess = false, ErrorMessage = errorMessage };
}
