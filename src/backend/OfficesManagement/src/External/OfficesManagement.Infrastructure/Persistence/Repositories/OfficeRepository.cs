using System.Linq.Expressions;
using MongoDB.Driver;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Core.Models.Entities;
using OfficesManagement.Core.Models;
using OfficesManagement.Infrastructure.Persistence.Contexts;
using OfficesManagement.Infrastructure.Repositories;

namespace OfficesManagement.Infrastructure.Persistence.Repositories
{
    public class OfficeRepository : MongoRepository<Office>, IOfficeRepository
    {
        public OfficeRepository(MongoDbContext context) : base(context, "Offices") { }

        public async Task<List<Office>> GetActiveOfficesAsync() =>
            await _collection.Find(o => o.IsActive).ToListAsync();

        public async Task<List<Office>> GetOfficesByCityAsync(string city) =>
            await _collection.Find(o => o.Location.City == city).ToListAsync();

        public async Task<List<Office>> GetOfficesByCountryAsync(string country) =>
            await _collection.Find(o => o.Location.Country == country).ToListAsync();

        public async Task<Office?> GetOfficeByNameAsync(string name) =>
            await _collection.Find(o => o.Name == name).FirstOrDefaultAsync();

        public async Task<List<Office>> GetFilteredAsync(Expression<Func<Office, bool>> filter) =>
            await _collection.Find(filter).ToListAsync();

        public async Task<List<Office>> GetPageAsync(PageSettings pageSettings) =>
            await _collection.Find(_ => true)
                              .Skip((pageSettings.PageIndex - 1) * pageSettings.PageSize)
                              .Limit(pageSettings.PageSize)
                              .ToListAsync();

        public async Task<int> GetOfficeCountAsync() =>
            (int)await _collection.CountDocumentsAsync(_ => true);
    }
}
