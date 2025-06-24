using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Domain.Models;
using AppointmentsManagement.Infrastructure.Persistense.Context;
using Microsoft.EntityFrameworkCore;
namespace AppointmentsManagement.Infrastructure.Persistense.Repositories;

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(AppointmentsManagementDbContext context)
        : base(context)
    {
    }

    public async Task<List<Appointment>> GetAllAsync()
    {
        List<Appointment> list = await _dbSet
            .Include(a => a.Result)
            .ToListAsync();

        return list;
    }

    public async Task<List<Appointment>> GetByPatientAsync(int patientId)
    {
        List<Appointment> list = await _dbSet
            .Where(a => a.PatientId == patientId)
            .Include(a => a.Result)
            .ToListAsync();

        return list;
    }

    public async Task<List<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateTime date)
    {
        List<Appointment> list = await _dbSet
            .Where(a => a.DoctorId == doctorId && a.Date.Date == date.Date)
            .Include(a => a.Result)
            .ToListAsync();

        return list;
    }

    public async Task<bool> ExistsByDoctorAndDateAsync(int doctorId, DateTime date)
    {
        bool exists = await _dbSet
            .AnyAsync(a => a.DoctorId == doctorId && a.Date == date);

        return exists;
    }

    public async Task<List<Appointment>> GetPendingByDoctorAsync(int doctorId)
    {
        List<Appointment> list = await _dbSet
            .Where(a => a.DoctorId == doctorId && !a.IsApproved)
            .Include(a => a.Result)
            .ToListAsync();

        return list;
    }

    public async Task<List<Appointment>> GetApprovedByPatientAsync(int patientId)
    {
        List<Appointment> list = await _dbSet
            .Where(a => a.PatientId == patientId && a.IsApproved)
            .Include(a => a.Result)
            .ToListAsync();

        return list;
    }

    public async Task<List<Appointment>> GetByServiceAsync(int serviceId)
    {
        List<Appointment> list = await _dbSet
            .Where(a => a.ServiceId == serviceId)
            .Include(a => a.Result)
            .ToListAsync();

        return list;
    }

    public async Task<List<Appointment>> GetUpcomingAsync(DateTime from, DateTime to)
    {
        List<Appointment> list = await _dbSet
            .Where(a => a.Date >= from && a.Date <= to)
            .Include(a => a.Result)
            .ToListAsync();

        return list;
    }
}
