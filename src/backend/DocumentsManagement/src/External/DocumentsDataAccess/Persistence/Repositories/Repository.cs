using DocumentsDataAccess.Persistence.Context;
using DocumentsDataAccess.Persistence.Interfaces.Auxiliary;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
namespace DocumentsDataAccess.Persistence.Repositories;

public class Repository<T>(AppDbContext _context) : IRepository<T> where T : class, ITrackable
{
    private readonly DbSet<T> _dbSet = _context.Set<T>();

    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _dbSet.ToListAsync(cancellationToken);
        var now = DateTime.UtcNow;

        foreach (var entity in list)
        {
            entity.LastRetrievedAt = now;
        }

        return list;
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);

        if (entity != null)
        {
            entity.LastRetrievedAt = DateTime.UtcNow;
        }

        return entity;
    }

    public async ValueTask AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.LastRetrievedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        entity.LastRetrievedAt = DateTime.UtcNow;
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

