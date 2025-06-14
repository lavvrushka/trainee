using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.SpecializationUseCases;

public record FilterSpecializationsByNameRequest(string Name): IRequest<IEnumerable<SpecializationDto>>;
public class FilterSpecializationsByNameHandler: IRequestHandler<FilterSpecializationsByNameRequest, IEnumerable<SpecializationDto>>
{
    private readonly ISpecializationRepository _repository;

    public FilterSpecializationsByNameHandler(ISpecializationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SpecializationDto>> Handle(FilterSpecializationsByNameRequest request, CancellationToken cancellationToken)
    {
        var list = await _repository.FilterByNameAsync(request.Name);

        return list.Select(s => s.MapSpecializationToDto());
    }
}