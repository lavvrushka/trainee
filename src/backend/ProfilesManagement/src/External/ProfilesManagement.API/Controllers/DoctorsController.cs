using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMediator.Interfaces;
using ProfilesManagement.API.Filters;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Application.UseCases.DoctorUseCases;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.API.Controllers;

[Route("api/doctor")]
[ApiController]
[Authorize]
public class DoctorController : ControllerBase
{
    private readonly IMediator _mediator;
    public DoctorController(IMediator mediator) => _mediator = mediator;

    [HttpPost("create")]
    [ValidateModel]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateDoctorRequest request)
    {
        var id = await _mediator.Send(request);

        return Ok(id);
    }

    [HttpPut("update")]
    [ValidateModel]
    public async Task<IActionResult> Update([FromBody] UpdateDoctorRequest request)
    {
        await _mediator.Send(request);

        return Ok();
    }

    [HttpGet("page")]
    public async Task<ActionResult<Pagination<DoctorDto>>> GetByPage(
            [FromQuery] GetAllDoctorsRequest request)
    {
        var page = await _mediator.Send(request);

        return Ok(page);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DoctorDto>> GetById(Guid id)
    {
        var dto = await _mediator.Send(new GetDoctorByIdRequest(id));

        return Ok(dto);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> Search([FromQuery] string name)
    {
        var list = await _mediator.Send(new FilterDoctorsByNameRequest(name));

        return Ok(list);
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteDoctorRequest(id));

        return NoContent();
    }
}
