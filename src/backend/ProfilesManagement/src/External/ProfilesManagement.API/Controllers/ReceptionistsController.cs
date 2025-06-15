using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMediator.Interfaces;
using ProfilesManagement.API.Filters;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Application.UseCases.ReceptionistUseCases;
using ProfilesManagement.Domain.Models;

namespace ProfilesManagement.API.Controllers;

[ApiController]
[Route("api/receptionist")]
[Authorize]
public class ReceptionistController : ControllerBase
{
    private readonly IMediator _mediator;
    public ReceptionistController(IMediator mediator) => _mediator = mediator;

    [HttpPost("create")]
    [ValidateModel]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateReceptionistRequest request)
    {
        var id = await _mediator.Send(request);

        return Ok(id);
    }

    [HttpPut("update")]
    [ValidateModel]
    public async Task<IActionResult> Update([FromBody] UpdateReceptionistRequest request)
    {
        await _mediator.Send(request);

        return Ok();
    }

    [HttpGet("page")]
    public async Task<ActionResult<Pagination<ReceptionistDto>>> GetByPage(
        [FromQuery] GetAllReceptionistsRequest request)
    {
        var page = await _mediator.Send(request);
        
        return Ok(page);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReceptionistDto>> GetById(Guid id)
    {
        var dto = await _mediator.Send(new GetReceptionistByIdRequest(id));

        return Ok(dto);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ReceptionistDto>>> Search(
        [FromQuery] FilterReceptionistsByNameRequest request)
    {
        var list = await _mediator.Send(request);

        return Ok(list);
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteReceptionistRequest(id));

        return NoContent();
    }
}
