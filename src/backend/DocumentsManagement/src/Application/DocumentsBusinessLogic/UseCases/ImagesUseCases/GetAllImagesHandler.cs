using DocumentsBusinessLogic.DTOs.Images;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using MyMediator.Interfaces;
namespace DocumentsBusinessLogic.UseCases.ImagesUseCases;

public record GetAllImagesRequest() : IRequest<IEnumerable<ImageDto>>;

public class GetAllImagesHandler : IRequestHandler<GetAllImagesRequest, IEnumerable<ImageDto>>
{
    private readonly IImageRepository _repository;

    public GetAllImagesHandler(IImageRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ImageDto>> Handle(GetAllImagesRequest request, CancellationToken ct)
    {
        var all = await _repository.GetAllAsync();

        return all.Where(e => !e.IsDeleted).Select(e => e.ToDto());
    }
}
