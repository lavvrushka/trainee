using DocumentsDataAccess.Persistence.Context;
using DocumentsDataAccess.Persistence.Entities;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
namespace DocumentsDataAccess.Persistence.Repositories;

public class DocumentRepository: Repository<DocumentEntity>, IDocumentRepository
{
    public DocumentRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<List<DocumentEntity>> ListMarkedDeletedAsync(DateTime cutoff)
    {
        return await _dbSet
            .Where(d => d.IsDeleted && d.LastRetrievedAt <= cutoff)
            .ToListAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        entity.IsDeleted = true;
        _dbSet.Update(entity);
    }

}
