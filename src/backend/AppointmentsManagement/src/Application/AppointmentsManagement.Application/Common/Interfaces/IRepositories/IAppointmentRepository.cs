using AppointmentsManagement.Domain.Models;
namespace AppointmentsManagement.Application.Common.Interfaces.IRepositories;

public interface IAppointmentRepository : IRepository<Appointment>
{
    public Task<List<Appointment>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default);
    public Task<List<Appointment>> GetByDoctorAndDateAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default);
    public Task<bool> ExistsByDoctorAndDateAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default);
    public Task<List<Appointment>> GetPendingByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default);
    public Task<List<Appointment>> GetApprovedByPatientAsync(Guid patientId, CancellationToken cancellationToken = default);
    public Task<List<Appointment>> GetByServiceAsync(Guid serviceId, CancellationToken cancellationToken = default);
    public Task<List<Appointment>> GetUpcomingAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}