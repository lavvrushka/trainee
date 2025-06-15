using DocumentsDataAccess.Persistence.Entities;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
namespace DocumentsDataAccess.Persistence.Repositories;

public interface IImageRepository : IRepository<ImageEntity>
{
    Task<List<ImageEntity>> ListMarkedDeletedAsync(DateTime cutoff);
    Task SoftDeleteAsync(Guid id);
}
