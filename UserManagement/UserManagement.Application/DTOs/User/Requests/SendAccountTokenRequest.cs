using MediatR;

namespace UserManagement.Application.DTOs.User.Requests
{
    public record SendAccountTokenRequest(string Email) : IRequest<Unit>;
}
