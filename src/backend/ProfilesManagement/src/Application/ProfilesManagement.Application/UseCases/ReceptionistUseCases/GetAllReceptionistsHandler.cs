using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.UseCases.ReceptionistUseCases;

public record GetAllReceptionistsRequest(int PageIndex, int PageSize): IRequest<Pagination<ReceptionistDto>>;

public class GetAllReceptionistsHandler: IRequestHandler<GetAllReceptionistsRequest, Pagination<ReceptionistDto>>
{
    private readonly IReceptionistRepository _repository;
    public GetAllReceptionistsHandler(IReceptionistRepository repository)
    {
        _repository = repository;
    }
    public async Task<Pagination<ReceptionistDto>> Handle(GetAllReceptionistsRequest request,CancellationToken cancellationToken)
    {
        var pageSettings = request.MapToPageSettings();
        List<Receptionist> list = await _repository.GetByPageAsync(pageSettings);
        int totalCount = await _repository.GetCountAsync();
        var domainPage = new Pagination<Receptionist>(list, totalCount, pageSettings);

        return domainPage.MapToReceptionistDtoPage();
    }
}
