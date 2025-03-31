using MediatR;
using UserManagement.Application.DTOs.User.Responses;

namespace UserManagement.Application.DTOs.User.Requests
{
    public record UserLoginRequest(
         string Email,
         string Password
     ) : IRequest<UserLoginResponse>;
}
