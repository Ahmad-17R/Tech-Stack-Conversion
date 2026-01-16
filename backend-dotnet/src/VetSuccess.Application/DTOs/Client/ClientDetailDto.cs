namespace VetSuccess.Application.DTOs.Client;

public class ClientDetailDto
{
    public string ClientOduId { get; set; } = string.Empty;
    public Guid Uuid { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? UpperFullName { get; set; }
    public string? PracticeOduId { get; set; }
    public string? PracticeName { get; set; }
    public bool? PimsIsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ExtractorRemovedAt { get; set; }
    public string? DataSource { get; set; }
    
    public List<EmailDto> Emails { get; set; } = new();
    public List<PhoneDto> Phones { get; set; } = new();
    public List<AddressDto> Addresses { get; set; } = new();
    public List<PatientDto> Patients { get; set; } = new();
    public List<AppointmentDto> Appointments { get; set; } = new();
}

public class EmailDto
{
    public string EmailOduId { get; set; } = string.Empty;
    public Guid Uuid { get; set; }
    public string? EmailAddress { get; set; }
    public bool? IsPreferred { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PhoneDto
{
    public string PhoneOduId { get; set; } = string.Empty;
    public Guid Uuid { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PhoneType { get; set; }
    public bool? IsPreferred { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AddressDto
{
    public string AddressOduId { get; set; } = string.Empty;
    public Guid Uuid { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PatientDto
{
    public string PatientOduId { get; set; } = string.Empty;
    public Guid Uuid { get; set; }
    public string? PatientName { get; set; }
    public string? Species { get; set; }
    public string? Breed { get; set; }
    public string? Sex { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool? IsDeceased { get; set; }
    public DateTime? DeceasedDate { get; set; }
    public string? OutcomeOduId { get; set; }
    public string? OutcomeName { get; set; }
    public DateTime? OutcomeAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AppointmentDto
{
    public string AppointmentOduId { get; set; } = string.Empty;
    public Guid Uuid { get; set; }
    public DateTime? AppointmentDate { get; set; }
    public string? AppointmentType { get; set; }
    public string? Status { get; set; }
    public string? PatientOduId { get; set; }
    public string? PatientName { get; set; }
    public DateTime CreatedAt { get; set; }
}
