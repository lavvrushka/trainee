namespace AppointmentsManagement.Domain.Models;

public class Result : IEntity
{
    public Guid Id { get; set; }

    public string Complaints { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
    public string Recommendations { get; set; } = string.Empty;

    public Guid AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
}