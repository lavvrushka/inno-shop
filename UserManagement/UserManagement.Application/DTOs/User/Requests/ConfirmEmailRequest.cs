using MediatR;

namespace UserManagement.Application.DTOs.User.Requests
{
    public record ConfirmEmailRequest(string Token) : IRequest<Unit>;

}
