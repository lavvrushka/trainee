using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Domain.Models;
using AppointmentsManagement.Infrastructure.Persistense.Context;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsManagement.Infrastructure.Persistense.Repositories;

public class Repository<T>(AppointmentsManagementDbContext _context) : IRepository<T> where T : class, IEntity
{
    private readonly DbSet<T> _dbSet = _context.Set<T>();

    public Task<Pagination<T>> GetAllAsync(PageSettings pageSettings, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        return PaginateAsync(query, pageSettings, cancellationToken);
    }
    protected static async Task<Pagination<T>> PaginateAsync(IQueryable<T> query, PageSettings pageSettings, CancellationToken cancellationToken)
    {
        int count = await query.CountAsync(cancellationToken);

        List<T> items = await query
            .Skip((pageSettings.PageIndex - 1) * pageSettings.PageSize)
            .Take(pageSettings.PageSize)
            .ToListAsync(cancellationToken);

        return new Pagination<T>(items, count, pageSettings);
    }

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async ValueTask AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
