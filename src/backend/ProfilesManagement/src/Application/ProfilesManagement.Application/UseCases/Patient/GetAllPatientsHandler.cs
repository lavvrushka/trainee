using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.UseCases.Patient;

public record GetAllPatientsRequest(int PageIndex, int PageSize)
    : IRequest<Pagination<PatientDto>>, IPageableRequest;

public class GetAllPatientsHandler : IRequestHandler<GetAllPatientsRequest, Pagination<PatientDto>>
{
    private readonly IPatientRepository _repository;

    public GetAllPatientsHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<Pagination<PatientDto>> Handle(GetAllPatientsRequest request, CancellationToken cancellationToken)
    {
        var pageSettings = request.MapToPageSettings();

        var totalCount = await _repository.GetCountAsync();
        if (totalCount == 0)
        {
            throw new Exception("No patients found.");
        }

        var patientsPage = await _repository.GetByPageAsync(pageSettings.PageIndex, pageSettings.PageSize);
        if (patientsPage == null || !patientsPage.Any())
        {
            throw new Exception("No patients found for the given page.");
        }

        var dtos = patientsPage.Select(p => p.MapToPatientDto()).ToList();

        return new Pagination<PatientDto>(dtos, totalCount, pageSettings);
    }