using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
namespace ProfilesManagement.Application.UseCases.PatientUseCases;

public record DeletePatientRequest(Guid Id) : IRequest<Unit>;

public class DeletePatientHandler : IRequestHandler<DeletePatientRequest, Unit>
{
    private readonly IPatientRepository _repository;

    public DeletePatientHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeletePatientRequest request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id);

        if (existing == null)
        {
            throw new KeyNotFoundException($"Patient with Id = {request.Id} not found.");
        }

        await _repository.DeleteAsync(existing);

        return Unit.Value;
    }
}
