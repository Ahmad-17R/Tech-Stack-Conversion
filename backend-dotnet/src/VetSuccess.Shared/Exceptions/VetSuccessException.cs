namespace VetSuccess.Shared.Exceptions;

/// <summary>
/// Base exception class for all VetSuccess application exceptions
/// </summary>
public abstract class VetSuccessException : Exception
{
    public string ErrorCode { get; }

    protected VetSuccessException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    protected VetSuccessException(string message, string errorCode, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
