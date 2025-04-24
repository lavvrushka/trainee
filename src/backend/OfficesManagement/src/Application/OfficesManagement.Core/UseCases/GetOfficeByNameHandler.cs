using MediatR;
using OfficesManagement.Core.Common.Exceptions;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Core.DTOs;
using OfficesManagement.Core.Mapper;
namespace OfficesManagement.Core.UseCases;

public record GetOfficeByNameRequest(string Name) : IRequest<OfficeDto>;

public class GetOfficeByNameHandler : IRequestHandler<GetOfficeByNameRequest, OfficeDto>
{
    private readonly IOfficeRepository _officeRepository;
    public GetOfficeByNameHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<OfficeDto> Handle(GetOfficeByNameRequest request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetOfficeByNameAsync(request.Name);

        if (office is null)
        {
            throw new NotFoundException($"Office with Name = '{request.Name}' was not found.");
        }

        return office.MapToOfficeDto();
    }
}
