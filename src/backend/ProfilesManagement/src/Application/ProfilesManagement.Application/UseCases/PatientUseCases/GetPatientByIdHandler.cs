using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.PatientUseCases;

public record GetPatientByIdRequest(Guid Id) : IRequest<PatientDto>;

public class GetPatientByIdHandler : IRequestHandler<GetPatientByIdRequest, PatientDto>
{
    private readonly IPatientRepository _repository;

    public GetPatientByIdHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<PatientDto> Handle(GetPatientByIdRequest request, CancellationToken cancellationToken)
    {
        var patient = await _repository.GetByIdAsync(request.Id);

        if (patient == null)
        {
            throw new KeyNotFoundException($"Patient with Id = {request.Id} not found.");
        }

        return patient.MapToPatientDto();
    }
}