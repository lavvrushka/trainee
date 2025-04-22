using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Filters;
using UserManagement.Application.DTOs;
using UserManagement.Application.UseCases.AccountTools;
using UserManagement.Application.UseCases.Auth;
using UserManagement.Application.UseCases.AuthUsecases;
using UserManagement.Application.UseCases.UserUsecases;
namespace UserManagement.API.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [ValidateModel]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        var result = await _mediator.Send(request);

        return Ok(result);
    }

    [HttpPost("register")]
    [ValidateModel]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
    {
        var result = await _mediator.Send(request);

        return Ok(result);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        _mediator.Send(new LogoutRequest());

        return NoContent();
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var userTokenResponse = await _mediator.Send(request);

        return Ok(userTokenResponse);
    }

    [HttpGet("current")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var request = new CurrentUserRequest();
        var userResponse = await _mediator.Send(request);

        return Ok(userResponse);
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<(List<UserDto> Users, int TotalCount)>> GetAllUsers(int pageNumber = 1, int pageSize = 3)
    {
        var request = new GetAllUsersRequest(pageNumber, pageSize);

        var users = await _mediator.Send(request);

        return Ok(users);
    }


    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        await _mediator.Send(request);

        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var request = new DeleteUserRequest(id);
        await _mediator.Send(request);

        return NoContent();
    }
}
