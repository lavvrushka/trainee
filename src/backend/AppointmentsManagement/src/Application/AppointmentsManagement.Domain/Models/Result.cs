namespace AppointmentsManagement.Domain.Models;

public class Result
{
    public int Id { get; set; }

    public string Complaints { get; set; }
    public string Conclusion { get; set; }
    public string Recommendations { get; set; }

    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; }
}