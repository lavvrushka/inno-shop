using MediatR;

namespace UserManagement.Application.DTOs.User.Requests
{
    public record RecoverAccountRequest(string Token) : IRequest<Unit>;
}
