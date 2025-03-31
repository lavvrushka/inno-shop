using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Application.UseCases.UserUsecases
{
    public class SetNewPasswordHandler : IRequestHandler<SetNewPasswordRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordResetService _passwordResetService;
        private readonly IHashPassword _hashPassword;

        public SetNewPasswordHandler(IUnitOfWork unitOfWork, IPasswordResetService passwordResetService, IHashPassword hashPassword)
        {
            _unitOfWork = unitOfWork;
            _passwordResetService = passwordResetService;
            _hashPassword = hashPassword;
        }

        public async Task<Unit> Handle(SetNewPasswordRequest request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.ConfirmPassword)
                throw new InvalidOperationException("Passwords do not match.");

            var user = await _unitOfWork.Users.GetByPasswordResetTokenAsync(request.Token);
            if (user == null)
                throw new InvalidOperationException("Invalid token.");

            var isValid = await _passwordResetService.ValidatePasswordResetTokenAsync(user.Id, request.Token);
            if (!isValid)
                throw new InvalidOperationException("Invalid or expired token.");

            var newPasswordHash = _hashPassword.Hash(request.NewPassword);
            await _unitOfWork.Users.ResetPasswordAsync(user.Id, newPasswordHash);

            await _unitOfWork.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
