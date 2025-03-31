using MediatR;

namespace UserManagement.Application.DTOs.User.Requests
{
    public record UserLogoutRequest : IRequest<Unit>;
}
