using DocumentsBusinessLogic.DTOs.Images;
using DocumentsDataAccess.Persistence.Entities;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using MyMediator.Interfaces;
namespace DocumentsBusinessLogic.UseCases.ImagesUseCases;

public record GetImageByIdRequest(
    Guid Id
) : IRequest<ImageDto>;

public class GetImageByIdHandler : IRequestHandler<GetImageByIdRequest, ImageDto>
{
    private readonly IImageRepository _repository;

    public GetImageByIdHandler(IImageRepository repository)
    {
        _repository = repository;
    }
    public async Task<ImageDto> Handle(GetImageByIdRequest request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
                    
        if (entity == null)
        {
            throw new KeyNotFoundException( $"Image with ID {request.Id} was not found.");
        }

        if (entity.IsDeleted)
        {
            throw new InvalidOperationException( $"Image {entity.Id} is marked as deleted.");
        }

        return entity.ToDto();
    }
}
