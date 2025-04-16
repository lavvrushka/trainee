using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Interfaces.Models;

namespace UserManagement.Application.UseCases.UserUsecases
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserRequest, Unit>
    {
        private readonly UserManager<Account> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailConfirmationService _emailConfirmationService;

        public UpdateUserHandler(
            UserManager<Account> userManager,
            ITokenService tokenService,
            IEmailConfirmationService emailConfirmationService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailConfirmationService = emailConfirmationService;
        }

        public async Task<Unit> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var userId = _tokenService.GetUserIdFromAccessToken();

            if (userId == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid token.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var oldEmail = user.Email;

            user.Email = request.Email;
            user.UserName = request.Email; 

            if (!string.Equals(oldEmail, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.EmailVerifiedAt = null;
                var newConfirmationToken = await _emailConfirmationService.GenerateEmailConfirmationTokenAsync(user.Id);
                user.EmailConfirmationToken = newConfirmationToken;
                await _emailConfirmationService.SendConfirmationEmailAsync(user.Id);
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to update user: {errors}");
            }

            return Unit.Value;
        }
    }
}
