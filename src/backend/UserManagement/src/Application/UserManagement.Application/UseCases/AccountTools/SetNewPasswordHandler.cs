using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Domain.Interfaces.Models;

namespace UserManagement.Application.UseCases.AccountTools
{
    public class SetNewPasswordHandler : IRequestHandler<SetNewPasswordRequest, Unit>
    {
        private readonly UserManager<Account> _userManager;

        public SetNewPasswordHandler(UserManager<Account> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> Handle(SetNewPasswordRequest request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.ConfirmPassword)
                throw new InvalidOperationException("Passwords do not match.");

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                throw new InvalidOperationException("User not found.");

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Password reset failed: {errors}");
            }

            return Unit.Value;
        }
    }
}
