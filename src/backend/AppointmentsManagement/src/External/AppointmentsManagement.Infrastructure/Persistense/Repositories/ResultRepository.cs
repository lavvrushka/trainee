using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Domain.Models;
using AppointmentsManagement.Infrastructure.Persistense.Context;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsManagement.Infrastructure.Persistense.Repositories;

public class ResultRepository(AppointmentsManagementDbContext context)
    : Repository<Result>(context), IResultRepository
{
    public Task<Result?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        return context.Results
            .Include(r => r.Appointment)
            .FirstOrDefaultAsync(r => r.AppointmentId == appointmentId, cancellationToken);
    }

    public Task<List<Result>> GetRecentAsync(int days, CancellationToken cancellationToken = default)
    {
        var fromDate = DateTime.UtcNow.AddDays(-days);

        return context.Results
            .Include(r => r.Appointment)
            .Where(r => r.Appointment.Date >= fromDate)
            .ToListAsync(cancellationToken);
    }
}