using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;

namespace ProfilesManagement.Application.UseCases.Patient;

public record UpdatePatientRequest(PatientDto Patient) : IRequest<Unit>;

public class UpdatePatientHandler : IRequestHandler<UpdatePatientRequest,Unit>
{
    private readonly IPatientRepository _repository;

    public UpdatePatientHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdatePatientRequest request, CancellationToken cancellationToken)
    {
        var dto = request.Patient;
        var existing = await _repository.GetByIdAsync(dto.Id);

        if (existing==null)
        {
             throw new KeyNotFoundException($"Patient with Id = {dto.Id} not found.");
        }

        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.MiddleName = dto.MiddleName;
        existing.AccountId = dto.AccountId;
        existing.ImageId = dto.ImageId;

        await _repository.UpdateAsync(existing);

        return Unit.Value;
    }
}