using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Application.UseCases.UserUsecases
{
    public class SendAccountTokenHandler : IRequestHandler<SendAccountTokenRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRecoveryService _accountRecoveryService;

        public SendAccountTokenHandler(IUnitOfWork unitOfWork, IAccountRecoveryService accountRecoveryService)
        {
            _unitOfWork = unitOfWork;
            _accountRecoveryService = accountRecoveryService;
        }

        public async Task<Unit> Handle(SendAccountTokenRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            await _accountRecoveryService.SendAccountRecoveryEmailAsync(user.Id);

            return Unit.Value;
        }
    }
}
