using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OfficesManagement.Core.Models.Entities;
using OfficesManagement.Infrastructure.Persistence.Configurations;

namespace OfficesManagement.Infrastructure.Persistence.Contexts
{
    public static class MongoDbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            MongoMappings.Register();

            using var scope = serviceProvider.CreateScope();
            var settings = scope.ServiceProvider.GetRequiredService<IOptions<MongoSettings>>().Value;
            var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

            var officeColl = context.Offices;
            var indexKeys = Builders<Office>.IndexKeys.Ascending(o => o.Name);
            var indexModel = new CreateIndexModel<Office>(indexKeys, new CreateIndexOptions { Unique = true });
            officeColl.Indexes.CreateOne(indexModel);
        }
    }
}
