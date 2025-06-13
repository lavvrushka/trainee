using System.Data;
using ProfilesManagement.Infrastructure.Persistence.Factories;
namespace ProfilesManagement.Infrastructure.Persistence.Repositories;

public abstract class DapperRepository<T> : IRepository<T> where T : class
{
    protected readonly IDbConnectionFactory _factory;

    protected DapperRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    protected IDbConnection Connection
    {
        get
        {
            var conn = _factory.CreateConnection();
            conn.Open();
            return conn;
        }
    }

    public abstract Task<List<T>> GetAllAsync();

    public abstract Task<T?> GetByIdAsync(Guid id);

    public abstract Task AddAsync(T entity);

    public abstract Task UpdateAsync(T entity);

    public abstract Task DeleteAsync(T entity);

}
