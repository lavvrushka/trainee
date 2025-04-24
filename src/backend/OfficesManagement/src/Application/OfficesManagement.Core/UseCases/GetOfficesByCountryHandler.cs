using MediatR;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Core.DTOs;
using OfficesManagement.Core.Mapper;
namespace OfficesManagement.Core.UseCases;

public record GetOfficesByCountryRequest(string Country) : IRequest<List<OfficeDto>>;

public class GetOfficesByCountryHandler : IRequestHandler<GetOfficesByCountryRequest, List<OfficeDto>>
{
    private readonly IOfficeRepository _officeRepository;
    public GetOfficesByCountryHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<List<OfficeDto>> Handle(GetOfficesByCountryRequest request, CancellationToken cancellationToken)
    {
        var offices = await _officeRepository.GetOfficesByCountryAsync(request.Country);
        return offices.Select(o => o.MapToOfficeDto()).ToList();
    }
}