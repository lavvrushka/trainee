using MediatR;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Core.DTOs;
using OfficesManagement.Core.Mapper;
namespace OfficesManagement.Core.UseCases;

public record GetActiveOfficesRequest() : IRequest<List<OfficeDto>>;

public class GetActiveOfficesHandler : IRequestHandler<GetActiveOfficesRequest, List<OfficeDto>>
{
    private readonly IOfficeRepository _officeRepository;
    public GetActiveOfficesHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<List<OfficeDto>> Handle(GetActiveOfficesRequest request, CancellationToken cancellationToken)
    {
        var offices = await _officeRepository.GetActiveOfficesAsync();
        return offices.Select(o => o.MapToOfficeDto()).ToList();
    }
}