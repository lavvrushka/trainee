using MediatR;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Core.DTOs;
using OfficesManagement.Core.Mapper;
using OfficesManagement.Core.Models;

namespace OfficesManagement.Core.UseCases;

public record GetAllOfficesQuery(
           int PageIndex,
           int PageSize
       ) : IRequest<Pagination<OfficeDto>>;
public class GetAllOfficesHandler : IRequestHandler<GetAllOfficesQuery, List<OfficeDto>>
{
    private readonly IOfficeRepository _officeRepository;

    public GetAllOfficesHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<List<OfficeDto>> Handle(GetAllOfficesQuery request, CancellationToken cancellationToken)
    {
        var offices = await _officeRepository.GetAllAsync();
        return offices.Select(o => o.MapToOfficeDto()).ToList();
    }
}
