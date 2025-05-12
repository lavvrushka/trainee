using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.UseCases.AccountTools;
namespace UserManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PasswordRecoveryController : ControllerBase
{
    private readonly IMediator _mediator;

    public PasswordRecoveryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("send-password-recovery-email")]
    public async Task<IActionResult> SendRecoveryEmail([FromBody] SendPasswordTokenRequest request)
    {
        await _mediator.Send(request);

        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] SetNewPasswordRequest request)
    {
        await _mediator.Send(request);

        return Ok();
    }
}
