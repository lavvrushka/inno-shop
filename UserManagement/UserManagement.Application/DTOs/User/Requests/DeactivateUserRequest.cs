using MediatR;
namespace UserManagement.Application.DTOs.User.Requests
{
    public record DeactivateUserRequest : IRequest<Unit>;
}
