using DocumentsDataAccess.Persistence.Entities;
namespace DocumentsDataAccess.Persistence.Interfaces.IRepositories;

public interface IDocumentRepository : IRepository<DocumentEntity>
{
    public Task<List<DocumentEntity>> ListMarkedDeletedAsync(DateTime cutoff);
    public Task SoftDeleteAsync(Guid id);
}
