using MediatR;
namespace UserManagement.Application.DTOs.Requests
{
    public record LogoutRequest : IRequest<Unit>;
}
