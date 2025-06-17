using DocumentsDataAccess.Persistence.Context;
using DocumentsDataAccess.Persistence.Entities;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
namespace DocumentsDataAccess.Persistence.Repositories;

public class ImageRepository: Repository<ImageEntity>, IImageRepository
{
    public ImageRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<List<ImageEntity>> ListMarkedDeletedAsync(DateTime cutoff)
    {
        return await _dbSet
            .Where(i => i.IsDeleted && i.LastRetrievedAt <= cutoff)
            .ToListAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        entity.IsDeleted = true;
        _dbSet.Update(entity);
    }
}
