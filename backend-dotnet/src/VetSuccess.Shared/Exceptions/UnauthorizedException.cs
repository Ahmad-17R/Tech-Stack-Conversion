namespace VetSuccess.Shared.Exceptions;

/// <summary>
/// Exception thrown when authentication fails or token is invalid
/// </summary>
public class UnauthorizedException : VetSuccessException
{
    public UnauthorizedException(string message) 
        : base(message, "UNAUTHORIZED")
    {
    }

    public UnauthorizedException(string message, Exception innerException) 
        : base(message, "UNAUTHORIZED", innerException)
    {
    }
}
