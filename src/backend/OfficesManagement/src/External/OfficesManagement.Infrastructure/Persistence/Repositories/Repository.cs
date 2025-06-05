using MongoDB.Driver;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Infrastructure.Persistence.Contexts;
namespace OfficesManagement.Infrastructure.Repositories;

public class MongoRepository<T> : IRepository<T> where T : class
{
    protected readonly IMongoCollection<T> _collection;

    public MongoRepository(MongoDbContext context, string collectionName)
    {
        _collection = context.GetCollection<T>(collectionName);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);

        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        var propertyInfo = entity.GetType().GetProperty("Id");
        var id = (Guid)propertyInfo.GetValue(entity, null)!;
        var filter = Builders<T>.Filter.Eq("Id", id);
        await _collection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(T entity)
    {
        var propertyInfo = entity.GetType().GetProperty("Id");
        var id = (Guid)propertyInfo.GetValue(entity, null)!;
        var filter = Builders<T>.Filter.Eq("Id", id);
        await _collection.DeleteOneAsync(filter);
    }
}
