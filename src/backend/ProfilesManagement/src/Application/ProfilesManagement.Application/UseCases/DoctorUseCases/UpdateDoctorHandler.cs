using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.UseCases.DoctorUseCases;

public record UpdateDoctorRequest(
    Guid Id,
    string FirstName,
    string LastName,
    string MiddleName,
    Guid AccountId,
    Guid OfficeId,
    Guid SpecializationId,
    DateTime CareerStartYear,
    Guid StatusId,
    Guid? ImageId
) : IRequest<Unit>;

public class UpdateDoctorHandler: IRequestHandler<UpdateDoctorRequest, Unit>
{
    private readonly IDoctorRepository _repository;

    public UpdateDoctorHandler(IDoctorRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateDoctorRequest request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id);

        if (existing == null)
        {
            throw new KeyNotFoundException($"Doctor {request.Id} not found");
        }
                     
        request.MapToDoctor(existing);
        await _repository.UpdateAsync(existing);

        return Unit.Value;
    }
}
