using MediatR;

namespace UserManagement.Application.DTOs.User.Requests
{
    public record UpdateUserRequest(string FirstName, string LastName, string Email) : IRequest<Unit>;
}
