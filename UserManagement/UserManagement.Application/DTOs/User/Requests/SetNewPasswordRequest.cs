using MediatR;

namespace UserManagement.Application.DTOs.User.Requests
{
    public record SetNewPasswordRequest(string Token, string NewPassword, string ConfirmPassword) : IRequest<Unit>;
}
