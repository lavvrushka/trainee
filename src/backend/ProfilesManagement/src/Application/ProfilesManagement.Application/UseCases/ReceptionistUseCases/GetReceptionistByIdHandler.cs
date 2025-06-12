using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.ReceptionistUseCases;

public record GetReceptionistByIdRequest(Guid Id) : IRequest<ReceptionistDto>;

public class GetReceptionistByIdHandler: IRequestHandler<GetReceptionistByIdRequest, ReceptionistDto>
{
    private readonly IReceptionistRepository _repository;
    public GetReceptionistByIdHandler(IReceptionistRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReceptionistDto> Handle(GetReceptionistByIdRequest request, CancellationToken cancellationToken)
    {
        var receptionist = await _repository.GetByIdAsync(request.Id);

        if (receptionist == null)
        {
            throw new KeyNotFoundException($"Receptionist {request.Id} not found");
        }

        return receptionist.MapToReceptionistDto();
    }
}
