using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.SpecializationUseCases;

public record UpdateSpecializationRequest(Guid Id, string Name, string Description): IRequest<Unit>;

public class UpdateSpecializationHandler: IRequestHandler<UpdateSpecializationRequest, Unit>
{
    private readonly ISpecializationRepository _repository;

    public UpdateSpecializationHandler(ISpecializationRepository repository)
    {
        _repository =  repository;
    }

    public async Task<Unit> Handle(UpdateSpecializationRequest request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id);

        if (existing == null)
        {
            throw new KeyNotFoundException($"Specialization {request.Id} not found");
        }

        request.MapSpecializationDtoToEntity(existing);
        await _repository.UpdateAsync(existing);

        return Unit.Value;
    }
}