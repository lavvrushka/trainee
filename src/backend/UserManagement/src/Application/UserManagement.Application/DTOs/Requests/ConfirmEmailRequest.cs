using MediatR;

namespace UserManagement.Application.DTOs.Requests
{
    public record ConfirmEmailRequest(string Token) : IRequest<Unit>;
}
