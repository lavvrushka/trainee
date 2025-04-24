using MediatR;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Core.DTOs.Requests;
using OfficesManagement.Core.Mapper;
namespace OfficesManagement.Core.UseCases;
public record CreateOfficeRequest(
        string Name,
        LocationRequest Location,
        bool IsActive,
        string RegistryPhoneNumber
) : IRequest<Unit>;
public class CreateOfficeHandler: IRequestHandler<CreateOfficeRequest, Unit>
{
    private readonly IOfficeRepository _officeRepository;

    public CreateOfficeHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<Unit> Handle(CreateOfficeRequest request, CancellationToken cancellationToken)
    {
        var officeEntity = request.MapToOffice();
        await _officeRepository.AddAsync(officeEntity);

        return Unit.Value;
    }
}

