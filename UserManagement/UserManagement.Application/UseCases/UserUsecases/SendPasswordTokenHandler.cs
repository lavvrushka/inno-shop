using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Application.UseCases.UserUsecases
{
    public class SendPasswordTokenHandler : IRequestHandler<SendPasswordTokenRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordResetService _passwordResetService;

        public SendPasswordTokenHandler(IUnitOfWork unitOfWork, IPasswordResetService passwordResetService)
        {
            _unitOfWork = unitOfWork;
            _passwordResetService = passwordResetService;
        }

        public async Task<Unit> Handle(SendPasswordTokenRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.email);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            await _passwordResetService.SendPasswordResetEmailAsync(user.Id);

            return Unit.Value;
        }
    }
}
