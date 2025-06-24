using AppointmentsManagement.Domain.Models;
namespace AppointmentsManagement.Application.Common.Interfaces.IRepositories;

public interface IAppointmentRepository : IRepository<Appointment>
{
    public Task<List<Appointment>> GetByPatientAsync(int patientId);
    public Task<List<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateTime date);
    public Task<bool> ExistsByDoctorAndDateAsync(int doctorId, DateTime date);
    public Task<List<Appointment>> GetPendingByDoctorAsync(int doctorId);
    public Task<List<Appointment>> GetApprovedByPatientAsync(int patientId);
    public Task<List<Appointment>> GetByServiceAsync(int serviceId);
    public Task<List<Appointment>> GetUpcomingAsync(DateTime from, DateTime to);
}