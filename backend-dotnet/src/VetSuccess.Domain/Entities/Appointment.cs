namespace VetSuccess.Domain.Entities;

public class Appointment : BaseCallCenterEntity
{
    public string AppointmentOduId { get; set; } = null!;
    public string? PatientOduId { get; set; }
    public Patient? Patient { get; set; }
    
    public DateTime? AppointmentDate { get; set; }
    public string? AppointmentType { get; set; }
    public string? Status { get; set; }
    public bool? IsCanceledAppointment { get; set; }
    public string? Description { get; set; }
}
