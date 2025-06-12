using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.EmploymentStatusUseCases;

public record UpdateEmploymentStatusRequest(
     Guid Id,
     string Status,
     string Description,
     DateTime DateTime
     ) : IRequest<Unit>;

public class UpdateEmploymentStatusHandler : IRequestHandler<UpdateEmploymentStatusRequest, Unit>
{
    private readonly IEmploymentStatusRepository _repository;

    public UpdateEmploymentStatusHandler(IEmploymentStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateEmploymentStatusRequest request, CancellationToken cancellationToken)
    {
        var employmentStatus = await _repository.GetByIdAsync(request.Id);

        if (employmentStatus == null)
        {
            throw new Exception($"EmploymentStatus with Id {request.Id} not found.");
        }

        request.MapEmploymentStatusDomain(employmentStatus);

        await _repository.UpdateAsync(employmentStatus);

        return Unit.Value;
    }
}
