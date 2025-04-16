using MediatR;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Application.UseCases.Auth
{
    public class LogoutHandler(ITokenService tokenService) : IRequestHandler<LogoutRequest, Unit>
    {
        private readonly ITokenService _tokenService = tokenService;
        public async Task<Unit> Handle(LogoutRequest _, CancellationToken cancellationToken)
        {
            await _tokenService.RevokeRefreshTokenAsync();
            return Unit.Value;
        }
    }
}
