using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.ResultUseCases;

public record GetResultByAppointmentRequest(Guid AppointmentId) : IRequest<ResultDto?>;

public class GetResultByAppointmentHandler : IRequestHandler<GetResultByAppointmentRequest, ResultDto?>
{
    private readonly IResultRepository _resultRepository;

    public GetResultByAppointmentHandler(IResultRepository resultRepository)
    {
        _resultRepository = resultRepository;
    }

    public async Task<ResultDto?> Handle(GetResultByAppointmentRequest request, CancellationToken cancellationToken)
    {
        var resultEntity = await _resultRepository
            .GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);

        if (resultEntity == null)
        {
            return null;
        }

        var resultDto = resultEntity.MapToDto();
        return resultDto;
    }
}
