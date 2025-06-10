using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.EmploymentStatusUseCases;
public record CreateEmploymentStatusRequest(
    string Status, 
    string Description, 
    DateTime DateTime
    ) : IRequest<Guid>;

public class CreateEmploymentStatusHandler : IRequestHandler<CreateEmploymentStatusRequest, Guid>
{
    private readonly IEmploymentStatusRepository _repository;

    public CreateEmploymentStatusHandler(IEmploymentStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateEmploymentStatusRequest request, CancellationToken cancellationToken)
    {
        var employmentStatus = request.MapEmploymentStatusDomain();
        await _repository.AddAsync(employmentStatus);

        return employmentStatus.Id;
    }
}
