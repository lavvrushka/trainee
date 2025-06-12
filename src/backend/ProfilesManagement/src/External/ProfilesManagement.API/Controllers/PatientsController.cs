using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMediator.Interfaces;
using ProfilesManagement.API.Filters;
namespace ProfilesManagement.API.Controllers;

[Route("api/patient")]
[ApiController]
[Authorize]
public class PatientController : ControllerBase
{
    private readonly IMediator _mediator;
    public PatientController(IMediator mediator) => _mediator = mediator;

    // POST api/patient/create
    [HttpPost("create")]
    [ValidateModel]
    public async Task<ActionResult<Guid>> Create([FromBody] CreatePatientRequest request)
    {
        var id = await _mediator.Send(request);
        return CreatedAtAction(null, id);
    }

    // PUT api/patient/update
    [HttpPut("update")]
    [ValidateModel]
    public async Task<IActionResult> Update([FromBody] UpdatePatientRequest request)
    {
        await _mediator.Send(request);
        return Ok();
    }
}