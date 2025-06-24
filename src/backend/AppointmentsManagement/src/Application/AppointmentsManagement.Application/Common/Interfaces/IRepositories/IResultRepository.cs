using AppointmentsManagement.Domain.Models;
namespace AppointmentsManagement.Application.Common.Interfaces.IRepositories;

public interface IResultRepository : IRepository<Result>
{
    public Task<Result?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    public Task<List<Result>> GetRecentAsync(int days, CancellationToken cancellationToken = default);
}
