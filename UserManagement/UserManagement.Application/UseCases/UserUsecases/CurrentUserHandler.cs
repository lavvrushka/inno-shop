using AutoMapper;
using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Application.DTOs.User.Responses;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Application.UseCases.UserUsecases
{
    public class CurrentUserHandler : IRequestHandler<CurrentUserRequest, UserResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public CurrentUserHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<UserResponse> Handle(CurrentUserRequest request, CancellationToken cancellationToken)
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

            var userResponse = _mapper.Map<UserResponse>(user);
            return userResponse;
        }
    }
}
