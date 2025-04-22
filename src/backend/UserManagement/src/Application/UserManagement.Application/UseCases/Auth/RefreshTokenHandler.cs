using MediatR;
using UserManagement.Application.DTOs.Responses;
using UserManagement.Domain.Interfaces.IServices;
namespace UserManagement.Application.UseCases.Auth;

public record RefreshTokenRequest(string RefreshToken) : IRequest<UserAuthResponse>;
public class RefreshTokenHandler(ITokenService tokenService) : IRequestHandler<RefreshTokenRequest, UserAuthResponse>
{
    private readonly ITokenService _tokenService = tokenService;

    public async Task<UserAuthResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var tokensPair = await _tokenService.RefreshTokensAsync(request.RefreshToken);

        return tokensPair.MapToUserAuthResponse();
    }
}
