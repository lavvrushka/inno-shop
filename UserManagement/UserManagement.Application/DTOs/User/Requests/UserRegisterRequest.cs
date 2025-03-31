using MediatR;
using UserManagement.Application.DTOs.User.Responses;

namespace UserManagement.Application.DTOs.User.Requests
{
    public record UserRegisterRequest
  (
       string Firstname,
       string Lastname,
       string Password,
       string Email,
       DateTime BirthDate

  ) : IRequest<UserRegisterResponse>;
}
