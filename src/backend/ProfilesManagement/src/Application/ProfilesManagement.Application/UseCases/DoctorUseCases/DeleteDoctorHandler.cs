using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
namespace ProfilesManagement.Application.UseCases.DoctorUseCases;

public record DeleteDoctorRequest(Guid Id) : IRequest<Unit>;

public class DeleteDoctorHandler: IRequestHandler<DeleteDoctorRequest, Unit>
{
    private readonly IDoctorRepository _repository;

    public DeleteDoctorHandler(IDoctorRepository repository)
    {
        _repository = repository;
    }
        
    public async Task<Unit> Handle( DeleteDoctorRequest request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id);

        if (existing == null)
        {
            throw new KeyNotFoundException($"Doctor {request.Id} not found");
        }
        await _repository.DeleteAsync(existing);

        return Unit.Value;
    }
}