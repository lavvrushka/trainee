using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
namespace ProfilesManagement.Application.UseCases.EmploymentStatusUseCases;

public record DeleteEmploymentStatusRequest(Guid Id) : IRequest<Unit>;

public class DeleteEmploymentStatusHandler : IRequestHandler<DeleteEmploymentStatusRequest, Unit>
{
    private readonly IEmploymentStatusRepository _repository;

    public DeleteEmploymentStatusHandler(IEmploymentStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteEmploymentStatusRequest request, CancellationToken cancellationToken)
    {
        var employmentStatus = await _repository.GetByIdAsync(request.Id);

        if (employmentStatus == null)
        {
            throw new Exception($"EmploymentStatus with Id {request.Id} not found.");
        }
        await _repository.DeleteAsync(employmentStatus);

        return Unit.Value;
    }
}