using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;


namespace AppointmentsManagement.Application.UseCases.ResultUseCases;
public record GetResultByIdRequest(Guid Id) : IRequest<ResultDto?>;
public class GetResultByIdHandler : IRequestHandler<GetResultByIdRequest, ResultDto?>
{
    private readonly IResultRepository _repository;

    public GetResultByIdHandler(IResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultDto?> Handle(GetResultByIdRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        return entity?.MapToDto();
    }
}
