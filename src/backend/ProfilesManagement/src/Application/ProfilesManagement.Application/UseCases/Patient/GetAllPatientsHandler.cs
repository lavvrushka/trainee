using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.Patient;

public record GetAllPatientsRequest(int PageNumber, int PageSize)
       : IRequest<(List<PatientDto> Patients, int TotalCount)>;

public class GetAllPatientsHandler : IRequestHandler<GetAllPatientsRequest, (List<PatientDto> Patients, int TotalCount)>
{
    private readonly IPatientRepository _repository;

    public GetAllPatientsHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<(List<PatientDto> Patients, int TotalCount)> Handle(GetAllPatientsRequest request, CancellationToken cancellationToken)
    {
        var allPatients = await _repository.GetAllAsync();

        if (allPatients == null || !allPatients.Any())
        {
            throw new Exception("No patients found.");
        }

        var totalCount = allPatients.Count;
        var paged = allPatients
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = paged.Select(p => p.MapToPatientDto()).ToList();

        return (dtos, totalCount);
    }
}