using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Domain.Models;
using AppointmentsManagement.Infrastructure.Persistense.Context;
using Microsoft.EntityFrameworkCore;
namespace AppointmentsManagement.Infrastructure.Persistense.Repositories;

public class ResultRepository : Repository<Result>, IResultRepository
{
    public ResultRepository(AppointmentsManagementDbContext context)
        : base(context)
    {
    }

    public async Task<Result?> GetByAppointmentIdAsync(int appointmentId)
    {
        Result? result = await _dbSet
            .Include(r => r.Appointment)
            .FirstOrDefaultAsync(r => r.AppointmentId == appointmentId);

        return result;
    }

    public async Task<List<Result>> GetByDoctorAsync(int doctorId)
    {
        List<Result> list = await _dbSet
            .Include(r => r.Appointment)
            .Where(r => r.Appointment.DoctorId == doctorId)
            .ToListAsync();

        return list;
    }

    public async Task<List<Result>> GetRecentAsync(int days)
    {
        DateTime cutoff = DateTime.UtcNow.AddDays(-days);

        List<Result> list = await _dbSet
            .Include(r => r.Appointment)
            .Where(r => r.Appointment.Date >= cutoff)
            .ToListAsync();

        return list;
    }
}