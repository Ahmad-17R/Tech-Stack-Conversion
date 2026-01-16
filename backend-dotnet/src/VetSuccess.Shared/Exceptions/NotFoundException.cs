namespace VetSuccess.Shared.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found
/// </summary>
public class NotFoundException : VetSuccessException
{
    public NotFoundException(string message) 
        : base(message, "NOT_FOUND")
    {
    }

    public NotFoundException(string resourceType, object resourceId) 
        : base($"{resourceType} with ID '{resourceId}' was not found", "NOT_FOUND")
    {
    }
}
