using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMediator.Interfaces;
using ProfilesManagement.API.Filters;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Application.UseCases.SpecializationUseCases;
namespace ProfilesManagement.API.Controllers;

[Route("api/specialization")]
[ApiController]
[Authorize(Policy = "Admin")]
public class SpecializationController : ControllerBase
{
    private readonly IMediator _mediator;

    public SpecializationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    [ValidateModel]
    public async Task<ActionResult<Guid>> Create(CreateSpecializationRequest req)
    {
        var id = await _mediator.Send(req);

        return Ok(id);
    }

    [HttpPut("update")]
    [ValidateModel]
    public async Task<IActionResult> Update(UpdateSpecializationRequest req)
    {
        await _mediator.Send(req);

        return Ok();
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<SpecializationDto>>> GetAll()
    {
        var list = await _mediator.Send(new GetAllSpecializationsRequest());

        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SpecializationDto>> GetById(Guid id)
    {
        var dto = await _mediator.Send(new GetSpecializationByIdRequest(id));

        return Ok(dto);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<SpecializationDto>>> Search([FromQuery] string name)
    {
        var results = await _mediator.Send(new FilterSpecializationsByNameRequest(name));

        return Ok(results);
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteSpecializationRequest(id));

        return NoContent();
    }
}
