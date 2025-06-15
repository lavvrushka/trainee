using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.DoctorUseCases;

public record FilterDoctorsBySpecializationRequest(Guid SpecializationId): IRequest<IEnumerable<DoctorDto>>;
public class FilterDoctorsBySpecializationHandler : IRequestHandler<FilterDoctorsBySpecializationRequest, IEnumerable<DoctorDto>>
{
    private readonly IDoctorRepository _repository;

    public FilterDoctorsBySpecializationHandler(IDoctorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DoctorDto>> Handle(FilterDoctorsBySpecializationRequest request, CancellationToken cancellationToken)
    {
        var doctors = await _repository.FilterBySpecializationAsync(request.SpecializationId);

        return doctors.Select(d => d.MapToDoctorDto());
    }
}