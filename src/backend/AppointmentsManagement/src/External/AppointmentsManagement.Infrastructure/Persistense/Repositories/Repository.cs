using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Infrastructure.Persistense.Context;
using Microsoft.EntityFrameworkCore;
namespace AppointmentsManagement.Infrastructure.Persistense.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppointmentsManagementDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppointmentsManagementDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
    }

}
