using MediatR;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Core.DTOs.Requests;
using OfficesManagement.Core.Models.Entities;
using OfficesManagement.Core.Models.ValueObjects;

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
    private readonly IMapper _mapper;

    public CreateOfficeHandler(IOfficeRepository officeRepository, IMapper mapper)
    {
        _officeRepository = officeRepository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateOfficeRequest request, CancellationToken cancellationToken)
    {

        var location = _mapper.Map<Location>(request.Location);
        var officeEntity = _mapper.Map<Office>(request);
        officeEntity.Location = location;
        await _officeRepository.AddAsync(officeEntity);
        return Unit.Value;
    }
}

