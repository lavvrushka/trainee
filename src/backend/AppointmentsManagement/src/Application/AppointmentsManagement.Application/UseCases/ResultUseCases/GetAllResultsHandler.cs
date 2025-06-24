using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Domain.Models;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.ResultUseCases;

public record GetAllResultsRequest(PageSettings PageSettings) : IRequest<Pagination<ResultDto>>;

public class GetAllResultsHandler : IRequestHandler<GetAllResultsRequest, Pagination<ResultDto>>
{
    private readonly IResultRepository _repository;

    public GetAllResultsHandler(IResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<Pagination<ResultDto>> Handle(GetAllResultsRequest request, CancellationToken cancellationToken)
    {
        Pagination<Result> pagedResults = await _repository.GetAllAsync(request.PageSettings, cancellationToken);

        List<ResultDto> dtos = pagedResults.Items.Select(result => result.MapToDto()).ToList();

        return new Pagination<ResultDto>(dtos,pagedResults.TotalCount,request.PageSettings);
    }
}
