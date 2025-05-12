using Microsoft.AspNetCore.Identity;
using UserManagement.Application.Common.Interfaces.IServices;
using UserManagement.Domain.Interfaces.Models;
namespace UserManagement.Infrastructure.Services;

public class PasswordResetService : IPasswordResetService
{
    private readonly UserManager<Account> _userManager;
    private readonly IEmailDeliveryService _emailDeliveryService;

    public PasswordResetService(UserManager<Account> userManager, IEmailDeliveryService emailDeliveryService)
    {
        _userManager = userManager;
        _emailDeliveryService = emailDeliveryService;
    }

    public async Task<string> GeneratePasswordResetTokenAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            throw new Exception("User not found.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        user.PasswordResetToken = token;
        user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1); 

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            throw new Exception($"Failed to store password reset token: {errors}");
        }

        return token;
    }

    public async Task SendPasswordResetEmailAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            throw new Exception("User not found.");

        var token = await GeneratePasswordResetTokenAsync(userId);
        var encodedToken = Uri.EscapeDataString(token);
        var encodedUserId = Uri.EscapeDataString(user.Id.ToString());

        var resetLink = $"http://localhost:3000/reset-password?userId={encodedUserId}&token={encodedToken}";

        var subject = "Reset Your Password";
        var body = $@"
                <p>Please reset your password by clicking on the link below:</p>
                <a href='{resetLink}'>Reset Password</a>
                <p>If you did not request this, you can safely ignore this email.</p>";

        await _emailDeliveryService.SendEmailAsync(user.Email, subject, body);
    }

    public async Task<bool> ResetPasswordAsync(Guid userId, string token, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return false;

        if (user.PasswordResetTokenExpires == null || user.PasswordResetTokenExpires < DateTime.UtcNow)
            return false;

        if (user.PasswordResetToken != token)
            return false;

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
            return false;

        user.PasswordResetToken = null;
        user.PasswordResetTokenExpires = null;
        await _userManager.UpdateAsync(user);

        return true;
    }
}
