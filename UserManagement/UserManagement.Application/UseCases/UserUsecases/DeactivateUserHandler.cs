using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;

public class DeactivateUserHandler : IRequestHandler<DeactivateUserRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IAccountRecoveryService _accountRecoveryService;
    private readonly IConnectionService _connectionService;

    public DeactivateUserHandler(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IAccountRecoveryService accountRecoveryService,
        IConnectionService connectionService) 
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _accountRecoveryService = accountRecoveryService;
        _connectionService = connectionService;
    }

    public async Task<Unit> Handle(DeactivateUserRequest request, CancellationToken cancellationToken)
    {
        var token = _tokenService.ExtractTokenFromHeader();
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException("Token is missing.");
        }

        var userId = _tokenService.ExtractUserIdFromToken(token);
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Invalid token.");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId.Value);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        if (user.EmailVerifiedAt == null)
        {
            throw new InvalidOperationException("Email must be confirmed before deactivating the account.");
        }

        await _accountRecoveryService.GenerateAccountRecoveryTokenAsync(user.Id);

        await _unitOfWork.Users.SetUserStatusAsync(user.Id, false);
        await _unitOfWork.SaveChangesAsync();

        var success = await _connectionService.HideUserProductsAsync(user.Id);
        if (!success)
        {
            throw new InvalidOperationException("Failed to hide products for the user.");
        }

        return Unit.Value;
    }
}
