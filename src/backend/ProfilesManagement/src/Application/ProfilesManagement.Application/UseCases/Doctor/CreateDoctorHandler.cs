using ProfilesManagement.Application.Common.Interfaces.IRepositories;
namespace ProfilesManagement.Application.UseCases.Doctor;

public class CreateDoctorHandler : IRequestHandler<CreateDoctorCommand, Guid>
{
    private readonly IDoctorRepository _repo;

    public CreateDoctorHandler(IDoctorRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> HandleAsync(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = new Doctor
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            AccountId = request.AccountId,
            OfficeId = request.OfficeId,
            SpecializationId = request.SpecializationId,
            CareerStartYear = request.CareerStartYear,
            Status = Enum.Parse<EmploymentStatus>(request.Status),
            ImageId = request.ImageId
        };

        return await _repo.AddAsync(doctor);
    }
}
