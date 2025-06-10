using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.PatientUseCases;

public record UpdatePatientRequest(
    Guid Id,
    string FirstName,
    string LastName,
    string MiddleName,
    Guid AccountId,
    Guid? ImageId
) : IRequest<Unit>;

public class UpdatePatientHandler : IRequestHandler<UpdatePatientRequest, Unit>
{
    private readonly IPatientRepository _repository;

    public UpdatePatientHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdatePatientRequest request, CancellationToken cancellationToken)
    {
        var patient = await _repository.GetByIdAsync(request.Id);

        if (patient == null)
        {
            throw new KeyNotFoundException($"Patient {request.Id} not found");
        }
        request.MapToPatient(patient);

        // ToDo: обновить сервис файлов или чета такое, если поменялась картинка (request.ImageId)

        await _repository.UpdateAsync(patient);

        return Unit.Value;
    }
}
