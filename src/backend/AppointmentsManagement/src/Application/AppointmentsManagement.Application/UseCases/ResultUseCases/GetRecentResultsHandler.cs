using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.ResultUseCases;

public record GetRecentResultsRequest(int Days) : IRequest<List<ResultDto>>;

public class GetRecentResultsHandler : IRequestHandler<GetRecentResultsRequest, List<ResultDto>>
{
    private readonly IResultRepository _resultRepository;

    public GetRecentResultsHandler(IResultRepository resultRepository)
    {
        _resultRepository = resultRepository;
    }

    public async Task<List<ResultDto>> Handle(GetRecentResultsRequest request, CancellationToken cancellationToken)
    {
        var resultEntities = await _resultRepository
            .GetRecentAsync(request.Days, cancellationToken);

        var resultDtos = new List<ResultDto>();
        foreach (var resultEntity in resultEntities)
        {
            resultDtos.Add(resultEntity.MapToDto());
        }

        return resultDtos;
    }
}
