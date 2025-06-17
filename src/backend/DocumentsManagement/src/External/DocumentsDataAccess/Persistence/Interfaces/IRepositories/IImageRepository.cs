using DocumentsDataAccess.Persistence.Entities;
namespace DocumentsDataAccess.Persistence.Interfaces.IRepositories;

public interface IImageRepository : IRepository<ImageEntity>
{
    Task<List<ImageEntity>> ListMarkedDeletedAsync(DateTime cutoff);
    Task SoftDeleteAsync(Guid id);
}

