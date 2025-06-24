namespace AppointmentsManagement.Domain.Models;

public class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int ServiceId { get; set; }

    public DateTime Date { get; set; }           
    public bool IsApproved { get; set; } = false;
    public Result Result { get; set; }
}