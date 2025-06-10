using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.UseCases.ReceptionistUseCases;

public record UpdateReceptionistRequest(
     Guid Id,
     string FirstName,
     string LastName,
     string MiddleName,
     EmploymentStatus Status,
     Guid AccountId,
     Guid? OfficeId,
     Guid? ImageId
 ) : IRequest<Unit>;

public class UpdateReceptionistHandler: IRequestHandler<UpdateReceptionistRequest, Unit>
{
    private readonly IReceptionistRepository _repository;
    public UpdateReceptionistHandler(IReceptionistRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateReceptionistRequest request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id);

        if (existing == null)
        {
            throw new KeyNotFoundException($"Receptionist {request.Id} not found");
        }
        request.MapToReceptionist(existing);
        await _repository.UpdateAsync(existing);

        return Unit.Value;
    }
}
