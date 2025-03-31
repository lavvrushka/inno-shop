using MediatR;

namespace UserManagement.Application.DTOs.User.Requests
{
    public record SendPasswordTokenRequest(string email) : IRequest<Unit>;
}
