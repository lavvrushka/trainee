using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagement.Domain.Interfaces.Models;
namespace UserManagement.Application.UseCases.UserUsecases;

public record DeleteUserRequest(Guid Id) : IRequest<Unit>;
public class DeleteUserHandler : IRequestHandler<DeleteUserRequest, Unit>
{
    private readonly UserManager<Account> _userManager;

    public DeleteUserHandler(UserManager<Account> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var user = await _userManager.FindByIdAsync(request.Id.ToString());

        if (user is null)
        {
            throw new InvalidOperationException("Пользователь не найден.");
        }

        var deleteResult = await _userManager.DeleteAsync(user);

        if (!deleteResult.Succeeded)
        {
            var errors = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
            throw new Exception($"Не удалось удалить пользователя: {errors}");
        }

        return Unit.Value;
    }
}
