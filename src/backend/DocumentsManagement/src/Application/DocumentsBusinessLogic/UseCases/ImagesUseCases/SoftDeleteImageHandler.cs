using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using MyMediator.Interfaces;
namespace DocumentsBusinessLogic.UseCases.ImagesUseCases;

public record SoftDeleteImageRequest(
     Guid Id
 ) : IRequest<Unit>;

public class SoftDeleteImageHandler : IRequestHandler<SoftDeleteImageRequest, Unit>
{
    private readonly IImageRepository _repository;

    public SoftDeleteImageHandler(IImageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(SoftDeleteImageRequest request, CancellationToken ct)
    {
        await _repository.SoftDeleteAsync(request.Id);

        return Unit.Value;
    }
}
