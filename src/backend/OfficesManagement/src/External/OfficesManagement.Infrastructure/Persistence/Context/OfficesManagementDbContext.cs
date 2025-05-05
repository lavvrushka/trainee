using MongoDB.Driver;
using Microsoft.Extensions.Options;
using OfficesManagement.Core.Models.Entities;
using OfficesManagement.Infrastructure.Persistence.Configurations;
namespace OfficesManagement.Infrastructure.Persistence.Contexts;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoSettings> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoDatabase Database => _database;
    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
    public IMongoCollection<Office> Offices => _database.GetCollection<Office>("Offices");
}
