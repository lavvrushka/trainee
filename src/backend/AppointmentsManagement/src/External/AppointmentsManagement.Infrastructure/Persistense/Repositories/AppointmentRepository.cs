using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Domain.Models;
using AppointmentsManagement.Infrastructure.Persistense.Context;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsManagement.Infrastructure.Persistense.Repositories;

public class AppointmentRepository(AppointmentsManagementDbContext context)
    : Repository<Appointment>(context), IAppointmentRepository
{
    public Task<List<Appointment>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return context.Appointments
            .Where(a => a.PatientId == patientId)
            .Include(a => a.Result)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Appointment>> GetByDoctorAndDateAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        return context.Appointments
            .Where(a => a.DoctorId == doctorId && a.Date >= start && a.Date < end)
            .Include(a => a.Result)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsByDoctorAndDateAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default)
    {
        return context.Appointments
            .AnyAsync(a => a.DoctorId == doctorId && a.Date == date, cancellationToken);
    }

    public Task<List<Appointment>> GetPendingByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        return context.Appointments
            .Where(a => a.DoctorId == doctorId && !a.IsApproved)
            .Include(a => a.Result)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Appointment>> GetApprovedByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return context.Appointments
            .Where(a => a.PatientId == patientId && a.IsApproved)
            .Include(a => a.Result)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Appointment>> GetByServiceAsync(Guid serviceId, CancellationToken cancellationToken = default)
    {
        return context.Appointments
            .Where(a => a.ServiceId == serviceId)
            .Include(a => a.Result)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Appointment>> GetUpcomingAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return context.Appointments
            .Where(a => a.Date >= from && a.Date <= to)
            .Include(a => a.Result)
            .ToListAsync(cancellationToken);
    }
}