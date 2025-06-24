using AppointmentsManagement.Domain.Models;
namespace AppointmentsManagement.Application.Common.Interfaces.IRepositories;

public interface IRepository<T> where T : class, IEntity
{
    public Task<Pagination<T>> GetAllAsync(PageSettings pageSettings, CancellationToken cancellationToken = default);
    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask AddAsync(T entity, CancellationToken cancellationToken = default);
    public void Update(T entity);
    public void Delete(T entity);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
