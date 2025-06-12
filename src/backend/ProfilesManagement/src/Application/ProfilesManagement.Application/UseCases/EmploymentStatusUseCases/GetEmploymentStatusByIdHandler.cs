using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
namespace ProfilesManagement.Application.UseCases.EmploymentStatusUseCases;

public record GetEmploymentStatusByIdRequest(Guid Id) : IRequest<EmploymentStatusDto>;

public class GetEmploymentStatusByIdHandler : IRequestHandler<GetEmploymentStatusByIdRequest, EmploymentStatusDto?>
{
    private readonly IEmploymentStatusRepository _repository;

    public GetEmploymentStatusByIdHandler(IEmploymentStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<EmploymentStatusDto> Handle(GetEmploymentStatusByIdRequest request, CancellationToken cancellationToken)
    {
        var employmentStatus = await _repository.GetByIdAsync(request.Id);

        if (employmentStatus == null)
        {
            throw new KeyNotFoundException($"EmploymentStatus with Id = {request.Id} not found.");
        }

        return employmentStatus.MapEmploymentStatusToDto();
    }
}