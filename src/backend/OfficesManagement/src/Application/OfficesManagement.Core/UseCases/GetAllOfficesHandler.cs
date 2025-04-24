using MediatR;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Core.DTOs;
using OfficesManagement.Core.Mapper;
using OfficesManagement.Core.Models;
namespace OfficesManagement.Core.UseCases;

public record GetAllOfficesRequest(
    int PageIndex,
    int PageSize
) : IRequest<Pagination<OfficeDto>>;

public class GetAllOfficesHandler : IRequestHandler<GetAllOfficesRequest, Pagination<OfficeDto>>
{
    private readonly IOfficeRepository _officeRepository;

    public GetAllOfficesHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<Pagination<OfficeDto>> Handle(GetAllOfficesRequest request, CancellationToken cancellationToken)
    {

        var pageSettings = request.MapToPageSettings();
        var offices = await _officeRepository.GetPageAsync(pageSettings);
        var totalCount = await _officeRepository.GetOfficeCountAsync();
        var officeDtos = offices.Select(o => o.MapToOfficeDto()).ToList();

        return new Pagination<OfficeDto>(officeDtos, totalCount, pageSettings);
    }
}
