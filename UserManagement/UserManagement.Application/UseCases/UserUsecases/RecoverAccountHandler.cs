using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Application.UseCases.UserUsecases
{
    public class RecoverAccountHandler : IRequestHandler<RecoverAccountRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRecoveryService _accountRecoveryService;
        private readonly IConnectionService _connectionService;
        public RecoverAccountHandler(IUnitOfWork unitOfWork, IAccountRecoveryService accountRecoveryService, IConnectionService connectionService)
        {
            _unitOfWork = unitOfWork;
            _accountRecoveryService = accountRecoveryService;
            _connectionService = connectionService;
        }

        public async Task<Unit> Handle(RecoverAccountRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByAccountRecoveryTokenAsync(request.Token);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid or expired recovery token.");
            }

            await _accountRecoveryService.RecoverAccountAsync(user.Id, request.Token);

            var success = await _connectionService.ShowUserProductsAsync(user.Id);
            if (!success)
            {
                throw new InvalidOperationException("Failed to show products for the user.");
            }

            return Unit.Value;
        }
    }
}
