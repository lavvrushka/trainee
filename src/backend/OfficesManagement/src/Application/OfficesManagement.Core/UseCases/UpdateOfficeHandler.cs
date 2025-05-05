using MediatR;
using OfficesManagement.Core.Common.Exceptions;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Core.DTOs.Requests;
using OfficesManagement.Core.Mapper;
namespace OfficesManagement.Core.UseCases;

public record UpdateOfficeRequest(
    Guid Id,
    string Name,
    LocationRequest? Location,
    bool IsActive,
    string RegistryPhoneNumber
) : IRequest<Unit>;

public class UpdateOfficeHandler : IRequestHandler<UpdateOfficeRequest, Unit>
{
    private readonly IOfficeRepository _officeRepository;

    public UpdateOfficeHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<Unit> Handle(UpdateOfficeRequest request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.Id);

        if (office is null)
        {
            throw new NotFoundException($"Office with Id = {request.Id} not found");
        }

        request.MapToOffice(office);
        await _officeRepository.UpdateAsync(office);

        return Unit.Value;
    }
}
