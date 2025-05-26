using Microsoft.EntityFrameworkCore;

namespace ProfilesManagement.Infrastructure.Persistence.Repositories;

public class ImageRepository : Repository<Image>, IImageRepository
{
    public ImageRepository(ProductManagementDbContext context) : base(context) { }

    public async Task<Guid> AddImageToProductAsync(Image image)
    {
        await _context.Set<Image>().AddAsync(image);
        return image.Id;
    }
}
