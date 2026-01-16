namespace VetSuccess.Application.DTOs.SMSHistory;

public class ContactedClientDto
{
    public Guid Uuid { get; set; }
    public string? ClientOduId { get; set; }
    public string? ClientName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? MessageText { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Status { get; set; }
    public string? PracticeOduId { get; set; }
    public string? PracticeName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ContactedClientsResponse
{
    public int Count { get; set; }
    public string? Next { get; set; }
    public string? Previous { get; set; }
    public List<ContactedClientDto> Results { get; set; } = new();
}
