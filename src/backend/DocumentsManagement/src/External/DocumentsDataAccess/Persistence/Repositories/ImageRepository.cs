using DocumentsDataAccess.Persistence.Context;
using DocumentsDataAccess.Persistence.Entities;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DocumentsDataAccess.Persistence.Repositories;

public class ImageRepository(AppDbContext context) : Repository<ImageEntity>(context), IImageRepository
{

    private readonly DbSet<ImageEntity> _dbSet = context.Set<ImageEntity>();
    public async Task<List<ImageEntity>> ListMarkedDeletedAsync(DateTime deletionCutoff)
    {
        return await _dbSet
            .Where(img => img.IsDeleted && img.LastRetrievedAt <= deletionCutoff)
            .ToListAsync();
    }

    public async Task SoftDeleteAsync(Guid imageId)
    {
        var image = await _dbSet.FindAsync(imageId);

        image.IsDeleted = true;
        image.LastRetrievedAt = DateTime.UtcNow;

        _dbSet.Update(image);
        await context.SaveChangesAsync();
    }
}
