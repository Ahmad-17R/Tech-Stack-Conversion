namespace VetSuccess.Shared.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class BusinessRuleException : VetSuccessException
{
    public BusinessRuleException(string message, string errorCode = "BUSINESS_RULE_VIOLATION") 
        : base(message, errorCode)
    {
    }

    public BusinessRuleException(string message, string errorCode, Exception innerException) 
        : base(message, errorCode, innerException)
    {
    }
}
