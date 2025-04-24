using MediatR;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
namespace OfficesManagement.Core.UseCases;

public record DeleteOfficeByIdRequest(Guid Id) : IRequest<Unit>;
public class DeleteOfficeByIdHandler : IRequestHandler<DeleteOfficeByIdRequest, Unit>
{
    private readonly IOfficeRepository _officeRepository;

    public DeleteOfficeByIdHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<Unit> Handle(DeleteOfficeByIdRequest request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.Id);

        if (office is null)
        {
            throw new KeyNotFoundException($"Office with ID '{request.Id}' was not found.");
        }

        await _officeRepository.DeleteAsync(office);

        return Unit.Value;
    }
}
