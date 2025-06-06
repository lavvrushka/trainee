using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.Patient;

public record CreatePatientRequest(
        string Name,
        string Description,
        decimal Price,
        int Quantity,
        bool IsAvailable,
        string ImageData,
        string ImageType
    ) : IRequest<PatientDto>;

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
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName,
            AccountId = dto.AccountId,
            ImageId = dto.ImageId
        };

        await _repository.AddAsync(patient);
        return patient.Id;
    }
}