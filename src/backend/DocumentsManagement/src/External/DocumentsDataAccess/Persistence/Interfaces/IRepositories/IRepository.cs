using DocumentsDataAccess.Persistence.Interfaces.Auxiliary;

namespace DocumentsDataAccess.Persistence.Interfaces.IRepositories;

public interface IRepository<T>where T : class, ITrackable
{
    public Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask AddAsync(T entity, CancellationToken cancellationToken = default);
    public void Update(T entity);
    public void Delete(T entity);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
