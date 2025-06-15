using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.DoctorUseCases;

public record FilterDoctorsByNameRequest(string Name): IRequest<IEnumerable<DoctorDto>>;

public class FilterDoctorsByNameHandler : IRequestHandler<FilterDoctorsByNameRequest, IEnumerable<DoctorDto>>
{
    private readonly IDoctorRepository _repo;

    public FilterDoctorsByNameHandler(IDoctorRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<DoctorDto>> Handle(FilterDoctorsByNameRequest request, CancellationToken cancellationToken)
    {
        var doctors = await _repo.FilterByNameAsync(request.Name);

        return doctors.Select(d => d.MapToDoctorDto());
    }
}