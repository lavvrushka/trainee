using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMediator.Interfaces;
using ProfilesManagement.API.Filters;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Application.UseCases.PatientUseCases;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.API.Controllers;

[ApiController]
[Route("api/patient")]
[Authorize]
public class PatientController : ControllerBase
{
    private readonly IMediator _mediator;

    public PatientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    [ValidateModel]
    public async Task<IActionResult> Create([FromBody] CreatePatientRequest request)
    {
        var id = await _mediator.Send(request);
        return Ok(id);
    }

    [HttpGet("page")]
    public async Task<ActionResult<Pagination<PatientDto>>> GetByPage(
        [FromQuery] GetAllPatientsRequest request)
    {
        var page = await _mediator.Send(request);

        return Ok(page);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientDto>> GetById(Guid id)
    {
        var dto = await _mediator.Send(new GetPatientByIdRequest(id));

        return Ok(dto);
    }

    [HttpPut("update")]
    [ValidateModel]
    public async Task<IActionResult> Update([FromBody] UpdatePatientRequest request)
    {
        await _mediator.Send(request);

        return Ok();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeletePatientRequest(id));

        return NoContent();
    }
}
