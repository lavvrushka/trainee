using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagement.Application.Common.Interfaces.IServices;
using UserManagement.Application.Common.Exeptions;
using UserManagement.Domain.Interfaces.Models;
namespace UserManagement.Application.UseCases.AccountTools;

public record SendPasswordTokenRequest(string email) : IRequest<Unit>;
public class SendPasswordTokenHandler : IRequestHandler<SendPasswordTokenRequest, Unit>
{
    private readonly UserManager<Account> _userManager;
    private readonly IPasswordResetService _passwordResetService;

    public SendPasswordTokenHandler(UserManager<Account> userManager, IPasswordResetService passwordResetService)
    {
        _userManager = userManager;
        _passwordResetService = passwordResetService;
    }

    public async Task<Unit> Handle(SendPasswordTokenRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.email);

        if (user is null)
            throw new EntityNotFoundException("User not found.");

        await _passwordResetService.SendPasswordResetEmailAsync(user.Id);

        return Unit.Value;
    }
}
