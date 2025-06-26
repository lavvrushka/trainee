using DocumentsDataAccess.Persistence.Context;
using DocumentsDataAccess.Persistence.Entities;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
namespace DocumentsDataAccess.Persistence.Repositories;

public class DocumentRepository(AppDbContext context) : Repository<DocumentEntity>(context), IDocumentRepository
{

    private readonly DbSet<DocumentEntity> _dbSet = context.Set<DocumentEntity>();

    public async Task<List<DocumentEntity>> ListMarkedDeletedAsync(DateTime deletionCutoff)
    {
        return await _dbSet
            .Where(doc => doc.IsDeleted && doc.LastRetrievedAt <= deletionCutoff)
            .ToListAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
       
        entity.IsDeleted = true;
        entity.LastRetrievedAt = DateTime.UtcNow;

        _dbSet.Update(entity);
        await context.SaveChangesAsync();
    }
}
