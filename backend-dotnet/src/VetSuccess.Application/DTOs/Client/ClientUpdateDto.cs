namespace VetSuccess.Application.DTOs.Client;

public class ClientUpdateDto
{
    public string? FullName { get; set; }
    public List<EmailUpdateDto>? Emails { get; set; }
    public List<PhoneUpdateDto>? Phones { get; set; }
    public List<PatientUpdateDto>? Patients { get; set; }
}

public class EmailUpdateDto
{
    public Guid? Uuid { get; set; }
    public string? EmailAddress { get; set; }
    public bool? IsPreferred { get; set; }
}

public class PhoneUpdateDto
{
    public Guid? Uuid { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PhoneType { get; set; }
    public bool? IsPreferred { get; set; }
}

public class PatientUpdateDto
{
    public Guid Uuid { get; set; }
    public string? OutcomeOduId { get; set; }
}
