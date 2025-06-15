using DocumentsDataAccess.Persistence.Entities;
namespace DocumentsDataAccess.Persistence.Interfaces.IRepositories;

public interface IDocumentRepository : IRepository<DocumentEntity>
{
    Task<List<DocumentEntity>> ListMarkedDeletedAsync(DateTime cutoff);
    Task SoftDeleteAsync(Guid id);
}
