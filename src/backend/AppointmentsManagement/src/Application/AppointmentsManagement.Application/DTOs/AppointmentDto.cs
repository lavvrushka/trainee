using AppointmentsManagement.Domain.Models;

namespace AppointmentsManagement.Application.DTOs;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }
    public DateTime Date { get; set; }
    public bool IsApproved { get; set; }
    public ResultDto? Result { get; set; }
}

public static class AppointmentMapper
{
    public static AppointmentDto MapToDto(this Appointment appt)
    {
        return new AppointmentDto
        {
            Id = appt.Id,
            PatientId = appt.PatientId,
            DoctorId = appt.DoctorId,
            ServiceId = appt.ServiceId,
            Date = appt.Date,
            IsApproved = appt.IsApproved,
            Result = appt.Result?.MapToDto()
        };
    }

    public static Appointment MapToEntity(this CreateAppointmentRequest req)
    {
        return new Appointment
        {
            PatientId = req.PatientId,
            DoctorId = req.DoctorId,
            ServiceId = req.ServiceId,
            Date = req.Date,
            IsApproved = false
        };
    }

    public static void MapToEntity(this UpdateAppointmentRequest req, Appointment appt)
    {
        appt.PatientId = req.PatientId;
        appt.DoctorId = req.DoctorId;
        appt.ServiceId = req.ServiceId;
        appt.Date = req.Date;
        appt.IsApproved = req.IsApproved;
    }
}
