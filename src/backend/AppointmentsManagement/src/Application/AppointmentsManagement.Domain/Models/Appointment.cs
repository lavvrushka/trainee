namespace AppointmentsManagement.Domain.Models;

public class Appointment : IEntity
{
    public Guid Id { get; set; }

    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }

    public DateTime Date { get; set; }
    public bool IsApproved { get; set; } = false;

    public Result? Result { get; set; }
}