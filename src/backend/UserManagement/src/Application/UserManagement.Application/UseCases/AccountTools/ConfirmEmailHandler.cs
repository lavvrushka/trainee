using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Interfaces.Models;

namespace UserManagement.Application.UseCases.AccountTools
{
    public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailRequest, Unit>
    {
        private readonly UserManager<Account> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailConfirmationService _emailConfirmationService;

        public ConfirmEmailHandler(UserManager<Account> userManager, ITokenService tokenService, IEmailConfirmationService emailConfirmationService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailConfirmationService = emailConfirmationService;
        }

        public async Task<Unit> Handle(ConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var userId = _tokenService.GetUserIdFromAccessToken();

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var result = await _emailConfirmationService.ConfirmEmailAsync(userId, request.Token);
            return Unit.Value;
        }
    }
}
