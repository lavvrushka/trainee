using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.SpecializationUseCases;

public record CreateSpecializationRequest(string Name, string Description): IRequest<Guid>;
public class CreateSpecializationHandler: IRequestHandler<CreateSpecializationRequest, Guid>
{
    private readonly ISpecializationRepository _repository;

    public CreateSpecializationHandler(ISpecializationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateSpecializationRequest request, CancellationToken cancellationToken)
    {
        var entity = request.MapSpecializationDtoToEntity();
        await _repository.AddAsync(entity);

        return entity.Id;
    }
}