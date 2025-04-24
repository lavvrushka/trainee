using MediatR;
using OfficesManagement.Core.Common.Exceptions;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Core.DTOs;
using OfficesManagement.Core.Mapper;
namespace OfficesManagement.Core.UseCases;

public record GetOfficeByIdRequest(Guid Id) : IRequest<OfficeDto>;

public class GetOfficeByIdHandler : IRequestHandler<GetOfficeByIdRequest, OfficeDto>
{
    private readonly IOfficeRepository _officeRepository;

    public GetOfficeByIdHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<OfficeDto> Handle(GetOfficeByIdRequest request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.Id);

        if (office is null)
        {
            throw new NotFoundException($"Office with Id = {request.Id} was not found.");
        }

        return office.MapToOfficeDto();
    }
}
