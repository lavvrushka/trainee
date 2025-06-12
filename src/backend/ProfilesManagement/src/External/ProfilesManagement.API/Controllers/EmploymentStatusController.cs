using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMediator.Interfaces;
using ProfilesManagement.API.Filters;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Application.UseCases.EmploymentStatusUseCases;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.API.Controllers;

[Route("api/employment-status")]
[ApiController]
[Authorize(Policy = "Admin")]
public class EmploymentStatusController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmploymentStatusController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    [ValidateModel]
    public async Task<IActionResult> Create([FromBody] CreateEmploymentStatusRequest request)
    {
        var id = await _mediator.Send(request);

        return Ok(id);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _mediator.Send(new GetAllEmploymentStatusesRequest());

        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _mediator.Send(new GetEmploymentStatusByIdRequest(id));

        return Ok(dto);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Filter([FromQuery] string name)
    {
        var list = await _mediator.Send(new FilterEmploymentStatusByNameRequest(name));

        return Ok(list);
    }

    [HttpPut("update")]
    [ValidateModel]
    public async Task<IActionResult> Update([FromBody] UpdateEmploymentStatusRequest request)
    {
        await _mediator.Send(request);

        return Ok();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteEmploymentStatusRequest(id));

        return NoContent();
    }
}