using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.SpecializationUseCases;

public record GetSpecializationByIdRequest(Guid Id): IRequest<SpecializationDto?>;
public class GetSpecializationByIdHandler : IRequestHandler<GetSpecializationByIdRequest, SpecializationDto?>
{
    private readonly ISpecializationRepository _repository;

    public GetSpecializationByIdHandler(ISpecializationRepository repository)
    {
        _repository = repository;
    }
    public async Task<SpecializationDto?> Handle(GetSpecializationByIdRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);

        return entity.MapSpecializationToDto();
    }
}