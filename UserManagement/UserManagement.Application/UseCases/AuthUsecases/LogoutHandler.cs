using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Application.UseCases.AuthUsecases
{
    public class LogoutHandler : IRequestHandler<UserLogoutRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public LogoutHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<Unit> Handle(UserLogoutRequest request, CancellationToken cancellationToken)
        {
            var token = _tokenService.ExtractTokenFromHeader() ?? throw new UnauthorizedAccessException("Token is missing.");
            var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(token);

            if (refreshToken != null)
            {
                await _tokenService.RevokeRefreshTokenAsync();
            }

            return Unit.Value;
        }
    }
}
