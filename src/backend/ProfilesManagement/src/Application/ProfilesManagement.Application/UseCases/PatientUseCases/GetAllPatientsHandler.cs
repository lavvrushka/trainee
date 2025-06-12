using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.UseCases.PatientUseCases;

public record GetAllPatientsRequest(
    int PageIndex,
    int PageSize
) : IRequest<Pagination<PatientDto>>;

public class GetAllPatientsHandler : IRequestHandler<GetAllPatientsRequest, Pagination<PatientDto>>
{
    private readonly IPatientRepository _repository;

    public GetAllPatientsHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<Pagination<PatientDto>> Handle( GetAllPatientsRequest request, CancellationToken cancellationToken)
    {

        var pageSettings = request.MapToPageSettings();
        var patients = await _repository.GetByPageAsync(pageSettings);
        var totalCount = await _repository.GetCountAsync();
        var domainPage = new Pagination<Patient>(patients, totalCount, pageSettings);
        var dtoPage = domainPage.MapToPatientDtoPage();

        return dtoPage;
    }
}
