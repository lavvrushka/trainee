using MongoDB.Driver;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Infrastructure.Persistence.Contexts;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly IMongoCollection<T> _collection;

    public Repository(MongoDbContext context)
    {
        var collectionName = typeof(T).Name + "s";
        _collection = context.Database.GetCollection<T>(collectionName);
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
        var idProp = entity.GetType().GetProperty("Id");
        if (idProp == null) throw new InvalidOperationException("Missing Id property");
        var id = (Guid)idProp.GetValue(entity)!;
        await _collection.ReplaceOneAsync(
            Builders<T>.Filter.Eq("Id", id),
            entity);
    }

    public async Task DeleteAsync(T entity)
    {
        var idProp = entity.GetType().GetProperty("Id");
        if (idProp == null) throw new InvalidOperationException("Missing Id property");
        var id = (Guid)idProp.GetValue(entity)!;
        await _collection.DeleteOneAsync(
            Builders<T>.Filter.Eq("Id", id));
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}