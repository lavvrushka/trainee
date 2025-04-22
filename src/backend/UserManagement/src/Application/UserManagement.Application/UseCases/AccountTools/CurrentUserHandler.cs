using MediatR;
using UserManagement.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using UserManagement.Application.Common.Exeptions;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Interfaces.Models;
namespace UserManagement.Application.UseCases.AccountTools;

public record CurrentUserRequest() : IRequest<UserDto>;
public class CurrentUserHandler : IRequestHandler<CurrentUserRequest, UserDto>
{
    private readonly UserManager<Account> _userManager;
    private readonly ITokenService _tokenService;

    public CurrentUserHandler(UserManager<Account> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<UserDto> Handle(CurrentUserRequest request, CancellationToken cancellationToken)
    {
        var userId = _tokenService.GetUserIdFromAccessToken();

        if (userId == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            throw new EntityNotFoundException("User not found.");
        }

        return user.MapToUserDto();
    }
}
