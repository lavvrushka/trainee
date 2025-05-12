using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Application.DTOs.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagement.Domain.Interfaces.Models;
using UserManagement.Application.Common.Exeptions;
namespace UserManagement.Application.UseCases.Auth;

public record UserLoginRequest(
 string Email,
 string Password) : IRequest<UserAuthResponse>;

public class LoginHandler(
    UserManager<Account> userManager,
    ITokenService tokenService) : IRequestHandler<UserLoginRequest, UserAuthResponse>
{
    private readonly UserManager<Account> _userManager = userManager;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<UserAuthResponse> Handle(UserLoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Email);

        if (user is null)
        {
            throw new InvalidCredentialsException("Неверные учетные данные.");
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new InvalidCredentialsException("Неверные учетные данные.");
        }

        var tokensPair = await _tokenService.GenerateTokensPairAsync(user);

        return tokensPair.MapToUserAuthResponse();
    }
}
