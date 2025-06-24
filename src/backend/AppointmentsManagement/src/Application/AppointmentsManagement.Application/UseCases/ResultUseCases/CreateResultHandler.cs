using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.ResultUseCases;
public record CreateResultRequest(
    string Complaints,
    string Conclusion,
    string Recommendations,
    Guid AppointmentId
) : IRequest<Guid>;
public class CreateResultHandler : IRequestHandler<CreateResultRequest, Guid>
{
    private readonly IResultRepository _repository;

    public CreateResultHandler(IResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateResultRequest request, CancellationToken cancellationToken)
    {
        var entity = request.MapToEntity();
        entity.Id = Guid.NewGuid();

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
