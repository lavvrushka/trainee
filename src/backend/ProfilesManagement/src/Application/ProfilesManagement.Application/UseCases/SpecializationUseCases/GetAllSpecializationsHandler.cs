using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.SpecializationUseCases;

public record GetAllSpecializationsRequest(): IRequest<IEnumerable<SpecializationDto>>;
public class GetAllSpecializationsHandler: IRequestHandler<GetAllSpecializationsRequest, IEnumerable<SpecializationDto>>
{
    private readonly ISpecializationRepository _repository;

    public GetAllSpecializationsHandler(ISpecializationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SpecializationDto>> Handle(GetAllSpecializationsRequest request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetAllAsync();

        return list.Select(s => s.MapSpecializationToDto());
    }
}