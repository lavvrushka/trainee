using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.PatientUseCases;

public record FilterPatientsByNameRequest(string Name) : IRequest<IEnumerable<PatientDto>>;

public class FilterPatientsByNameHandler : IRequestHandler<FilterPatientsByNameRequest, IEnumerable<PatientDto>>
{
    private readonly IPatientRepository _repository;

    public FilterPatientsByNameHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PatientDto>> Handle(FilterPatientsByNameRequest request, CancellationToken cancellationToken)
    {
        var patients = await _repository.SearchByNameAsync(request.Name);

        return patients.Select(p => p.MapToPatientDto());
    }
}