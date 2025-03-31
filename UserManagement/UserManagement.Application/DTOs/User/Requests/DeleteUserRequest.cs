using MediatR;
namespace UserManagement.Application.DTOs.User.Requests
{
    public record DeleteUserRequest(Guid Id) : IRequest<Unit>;
}
