namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface IImageRepository : IRepository<Image>
{
    Task<Guid> AddImageToObjectAsync(Image entity);
}
