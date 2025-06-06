using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.Patient;

public record CreatePatientRequest() : IRequest<PatientDto>;

public class CreatePatientHandler : IRequestHandler<CreatePatientRequest, Guid>
{
    private readonly IPatientRepository _repository;

    public CreatePatientHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreatePatientRequest request, CancellationToken cancellationToken)
    {
        var dto = request.Patient;
        var patient = new Patient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName,
        };

        await _repository.AddAsync(patient);
        return patient.Id;
    }
}