using DocumentsDataAccess.Persistence.Context;
using DocumentsDataAccess.Persistence.Interfaces.Auxiliary;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
namespace DocumentsDataAccess.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class, ITrackable
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        var list = await _dbSet.ToListAsync();
        var now = DateTime.UtcNow;
        foreach (var entity in list)
        {
            entity.LastRetrievedAt = now;
        }
        await _context.SaveChangesAsync();

        return list;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);

        if (entity != null)
        {
            entity.LastRetrievedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return entity;
    }

    public virtual async Task AddAsync(T entity)
    {
        entity.LastRetrievedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        entity.LastRetrievedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}