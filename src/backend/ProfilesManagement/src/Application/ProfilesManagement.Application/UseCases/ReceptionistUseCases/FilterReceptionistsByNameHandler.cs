using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.ReceptionistUseCases;

public record FilterReceptionistsByNameRequest(string Name): IRequest<IEnumerable<ReceptionistDto>>;
public class FilterReceptionistsByNameHandler: IRequestHandler<FilterReceptionistsByNameRequest, IEnumerable<ReceptionistDto>>
{
    private readonly IReceptionistRepository _repository;

    public FilterReceptionistsByNameHandler(IReceptionistRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReceptionistDto>> Handle( FilterReceptionistsByNameRequest request, CancellationToken cancellationToken)
    {
        var entities = await _repository.FilterByNameAsync(request.Name);

        return entities.Select(r => r.MapToReceptionistDto());
    }
}