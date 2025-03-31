using AutoMapper;
using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Application.DTOs.User.Responses;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Application.UseCases.AuthUsecases
{
    public class LoginHandler(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IHashPassword hashPassword,
        IMapper mapper) : IRequestHandler<UserLoginRequest, UserLoginResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IHashPassword _hashPassword = hashPassword;
        private readonly IMapper _mapper = mapper;

        public async Task<UserLoginResponse> Handle(UserLoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (user == null || !_hashPassword.VerifyPassword(user.Password, request.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            bool status = await _unitOfWork.Users.IsUserActiveAsync(user.Id);
            if (!status)
            {
                throw new UnauthorizedAccessException("User account is deactivated.");
            }
            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

            return _mapper.Map<UserLoginResponse>((accessToken, refreshToken));
        }

    }
}
