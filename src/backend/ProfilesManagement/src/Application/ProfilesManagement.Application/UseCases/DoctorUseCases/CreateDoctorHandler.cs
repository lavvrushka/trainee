using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.UseCases.DoctorUseCases;

public record CreateDoctorRequest(
      string FirstName,
      string LastName,
      string MiddleName,
      Guid AccountId,
      Guid? OfficeId,
      Guid? SpecializationId,
      DateTime CareerStartYear,
      EmploymentStatus Status,
      Guid? ImageId
  ) : IRequest<Guid>;

public class CreateDoctorHandler: IRequestHandler<CreateDoctorRequest, Guid>
{
    private readonly IDoctorRepository _repository;

    public CreateDoctorHandler(IDoctorRepository repository)
    {
        _repository = repository;
    }
    public async Task<Guid> Handle(CreateDoctorRequest request,CancellationToken cancellationToken)
    {
        var doctor = request.MapToDoctor();
        await _repository.AddAsync(doctor);

        return doctor.Id;
    }
}
