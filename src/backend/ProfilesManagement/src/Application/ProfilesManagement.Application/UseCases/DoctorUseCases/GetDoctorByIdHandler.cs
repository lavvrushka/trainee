using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.DoctorUseCases;

public record GetDoctorByIdRequest(Guid Id): IRequest<DoctorDto>;

public class GetDoctorByIdHandler: IRequestHandler<GetDoctorByIdRequest, DoctorDto>
{
    private readonly IDoctorRepository _repository;

    public GetDoctorByIdHandler(IDoctorRepository repository)
    {
        _repository = repository;
    }
    public async Task<DoctorDto> Handle(GetDoctorByIdRequest request, CancellationToken cancellationToken)
    {
        var doctor = await _repository.GetByIdAsync(request.Id);

        if (doctor == null)
        {
            throw new KeyNotFoundException($"Doctor {request.Id} not found");
        }
              
        return doctor.MapToDoctorDto();
    }
}
