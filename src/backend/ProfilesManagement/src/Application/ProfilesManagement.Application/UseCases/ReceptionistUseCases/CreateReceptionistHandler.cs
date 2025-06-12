using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.UseCases.ReceptionistUseCases;

public record CreateReceptionistRequest(
        string FirstName,
        string LastName,
        string MiddleName,
        EmploymentStatus Status,
        Guid AccountId,
        Guid? OfficeId,
        Guid? ImageId
    ) : IRequest<Guid>;

public class CreateReceptionistHandler: IRequestHandler<CreateReceptionistRequest, Guid>
{
    private readonly IReceptionistRepository _repository;
    public CreateReceptionistHandler(IReceptionistRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Guid> Handle(CreateReceptionistRequest request,CancellationToken cancellationToken)
    {
        var receptionist = request.MapToReceptionist();
        await _repository.AddAsync(receptionist);

        return receptionist.Id;
    }
}
