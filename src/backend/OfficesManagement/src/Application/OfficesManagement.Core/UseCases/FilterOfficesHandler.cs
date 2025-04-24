using System.Linq.Expressions;
using MediatR;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Core.DTOs;
using OfficesManagement.Core.Mapper;
using OfficesManagement.Core.Models.Entities;  
namespace OfficesManagement.Core.UseCases;

public record FilterOfficesRequest(
    string? Address,
    string? City,
    string? Country,
    string? IsActive
) : IRequest<List<OfficeDto>>;

public class FilterOfficesHandler : IRequestHandler<FilterOfficesRequest, List<OfficeDto>>
{
    private readonly IOfficeRepository _officeRepository;

    public FilterOfficesHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<List<OfficeDto>> Handle(FilterOfficesRequest request,CancellationToken cancellationToken)
    {
        Expression<Func<Office, bool>> filter = o =>
            (string.IsNullOrWhiteSpace(request.Address)
                 || o.Location.Address.Contains(request.Address!)) &&
            (string.IsNullOrWhiteSpace(request.City)
                 || o.Location.City.Equals(request.City!)) &&
            (string.IsNullOrWhiteSpace(request.Country)
                 || o.Location.Country.Equals(request.Country!)) &&
            (string.IsNullOrWhiteSpace(request.IsActive)
                 || o.IsActive.ToString()
                     .Equals(request.IsActive!, StringComparison.OrdinalIgnoreCase));

        var offices = await _officeRepository.GetFilteredAsync(filter);

        return offices.Select(o => o.MapToOfficeDto()).ToList();
    }
}
