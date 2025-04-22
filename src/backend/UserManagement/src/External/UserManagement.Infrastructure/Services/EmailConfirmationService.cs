using Microsoft.AspNetCore.Identity;
using UserManagement.Application.Common.Interfaces.IServices;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Interfaces.Models;
namespace UserManagement.Infrastructure.Services;

public class EmailConfirmationService : IEmailConfirmationService
{
    private readonly UserManager<Account> _userManager;
    private readonly IEmailDeliveryService _emailService;

    public EmailConfirmationService(
        UserManager<Account> userManager,
        IEmailDeliveryService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            throw new Exception("User not found");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        return token;
    }

    public async Task SendConfirmationEmailAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            throw new Exception("User not found");

        var token = await GenerateEmailConfirmationTokenAsync(userId);
        var encodedToken = Uri.EscapeDataString(token);
        var confirmationLink = $"http://localhost:3000/confirm-email/{user.Id}?token={encodedToken}";

        var subject = "Confirm Your Email";
        var body = $"<p>Please confirm your email by clicking on the link below:</p>" +
                   $"<a href='{confirmationLink}'>Confirm Email</a>";

        await _emailService.SendEmailAsync(user.Email, subject, body);
    }

    public async Task<bool> ConfirmEmailAsync(Guid userId, string token)
    {

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return false;
        }

        var decodedToken = Uri.UnescapeDataString(token);

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return false;
        }

        user.EmailVerifiedAt = DateTime.UtcNow;
        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var updateErrors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            throw new Exception($"Failed to update user after email confirmation: {updateErrors}");
        }

        return true;
    }


}
