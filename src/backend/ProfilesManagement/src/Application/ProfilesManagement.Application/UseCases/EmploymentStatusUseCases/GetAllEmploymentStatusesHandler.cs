using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.EmploymentStatusUseCases;
public record GetAllEmploymentStatusesRequest() : IRequest<IEnumerable<EmploymentStatusDto>>;

public class GetAllEmploymentStatusesHandler : IRequestHandler<GetAllEmploymentStatusesRequest, IEnumerable<EmploymentStatusDto>>
{
    private readonly IEmploymentStatusRepository _repository;

    public GetAllEmploymentStatusesHandler(IEmploymentStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EmploymentStatusDto>> Handle(GetAllEmploymentStatusesRequest request, CancellationToken cancellationToken)
    {
        var statuses = await _repository.GetAllAsync();

        return statuses.Select(s => s.MapEmploymentStatusToDto());
    }
}