using MediatR;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Core.DTOs;
using OfficesManagement.Core.Mapper;
namespace OfficesManagement.Core.UseCases;

public record GetOfficesByCityRequest(string City) : IRequest<List<OfficeDto>>;

public class GetOfficesByCityHandler : IRequestHandler<GetOfficesByCityRequest, List<OfficeDto>>
{
    private readonly IOfficeRepository _officeRepository;
    public GetOfficesByCityHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<List<OfficeDto>> Handle(GetOfficesByCityRequest request, CancellationToken cancellationToken)
    {
        var offices = await _officeRepository.GetOfficesByCityAsync(request.City);

        return offices.Select(o => o.MapToOfficeDto()).ToList();
    }
}
