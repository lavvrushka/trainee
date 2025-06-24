using AppointmentsManagement.Domain.Models;
namespace AppointmentsManagement.Application.Common.Interfaces.IRepositories;

public interface IResultRepository : IRepository<Result>
{
    public Task<Result?> GetByAppointmentIdAsync(int appointmentId);
    public Task<List<Result>> GetByDoctorAsync(int doctorId);
    public Task<List<Result>> GetRecentAsync(int days);
}
