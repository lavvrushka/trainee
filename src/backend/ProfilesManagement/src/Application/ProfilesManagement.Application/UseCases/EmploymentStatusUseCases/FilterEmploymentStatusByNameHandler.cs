using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.EmploymentStatusUseCases;
public record FilterEmploymentStatusByNameRequest(string Name) : IRequest<IEnumerable<EmploymentStatusDto>>;

public class FilterEmploymentStatusByNameHandler : IRequestHandler<FilterEmploymentStatusByNameRequest, IEnumerable<EmploymentStatusDto>>
{
    private readonly IEmploymentStatusRepository _repository;

    public FilterEmploymentStatusByNameHandler(IEmploymentStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EmploymentStatusDto>> Handle(FilterEmploymentStatusByNameRequest request, CancellationToken cancellationToken)
    {
        var statuses = await _repository.FilterByStatusNameAsync(request.Name);

        return statuses.Select(s => s.MapEmploymentStatusToDto());
    }
}
