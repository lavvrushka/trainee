namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface IRepository<T> where T : class
{
    public Task<List<T>> GetAllAsync();
    public Task<T?> GetByIdAsync(Guid id);
    public Task AddAsync(T entity);
    public Task UpdateAsync(T entity);
    public Task DeleteAsync(T entity);
    public Task<int> GetCountAsync();
}