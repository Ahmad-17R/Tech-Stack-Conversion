namespace VetSuccess.Shared.Constants;

/// <summary>
/// Error codes matching Django error message enums for consistency
/// </summary>
public static class ErrorCodes
{
    public const string X0001 = "X0001"; // Invalid ODU ID
    public const string X0002 = "X0002"; // Invalid query parameters
    public const string X0003 = "X0003"; // Invalid outcome value
    public const string X0005 = "X0005"; // SMS fields validation failed
    public const string X0007 = "X0007"; // Email fields validation failed
    
    public const string NotFound = "NOT_FOUND";
    public const string ValidationError = "VALIDATION_ERROR";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
}
