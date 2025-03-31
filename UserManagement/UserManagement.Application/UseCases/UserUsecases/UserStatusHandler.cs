using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Application.DTOs.User.Responses;
using UserManagement.Domain.Interfaces.IRepositories;

namespace UserManagement.Application.UseCases.UserUsecases
{
    public class UserStatusHandler : IRequestHandler<UserStatusRequest, UserStatusResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserStatusHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserStatusResponse> Handle(UserStatusRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return new UserStatusResponse(user.IsActive);
        }
    }
}
