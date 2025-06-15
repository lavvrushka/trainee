using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.DoctorUseCases;

public record FilterDoctorsByOfficeRequest(Guid OfficeId): IRequest<IEnumerable<DoctorDto>>;

public class FilterDoctorsByOfficeHandler: IRequestHandler<FilterDoctorsByOfficeRequest, IEnumerable<DoctorDto>>
{
    private readonly IDoctorRepository _repository;

    public FilterDoctorsByOfficeHandler(IDoctorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DoctorDto>> Handle(FilterDoctorsByOfficeRequest request, CancellationToken cancellationToken)
    {
        var doctors = await _repository.FilterByOfficeAsync(request.OfficeId);

        return doctors.Select(d => d.MapToDoctorDto());
    }
}