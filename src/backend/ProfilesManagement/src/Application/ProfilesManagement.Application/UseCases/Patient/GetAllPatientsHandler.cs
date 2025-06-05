using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;

namespace ProfilesManagement.Application.UseCases.Patient;

public record GetAllPatientsRequest(int PageNumber, int PageSize) : IRequest<(List<PatientDto> Patients, int TotalCount)>;

public class GetAllPatientsHandler : IRequestHandler<GetAllPatientsRequest, (List<PatientDto> Patients, int TotalCount)>
{
    private readonly IPatientRepository _repository;

    public GetAllPatientsHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<(List<PatientDto> Patients, int TotalCount)> HandleAsync(GetAllPatientsRequest request, CancellationToken cancellationToken)
    {
        var patients = await _repository.GetAllAsync();
        var totalCount = patients.Count;

        var paged = patients
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var result = paged.Select(p => p.ToDto()).ToList();

        return (result, totalCount);
    }
}
