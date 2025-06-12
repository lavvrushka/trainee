using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.UseCases.DoctorUseCases;

public record GetAllDoctorsRequest(
    int PageIndex,
    int PageSize
    ): IRequest<Pagination<DoctorDto>>;

public class GetAllDoctorsHandler: IRequestHandler<GetAllDoctorsRequest, Pagination<DoctorDto>>
{
    private readonly IDoctorRepository _repository;

    public GetAllDoctorsHandler(IDoctorRepository repository)
    {
        _repository = repository;
    }
    public async Task<Pagination<DoctorDto>> Handle(GetAllDoctorsRequest request, CancellationToken cancellationToken)
    {
        var pageSettings = request.MapToPageSettings();
        var list = await _repository.GetByPageAsync(pageSettings);
        var totalCount = await _repository.GetCountAsync();
        var domainPage = new Pagination<Doctor>(list, totalCount, pageSettings);

        return domainPage.MapToDoctorDtoPage();
    }
}