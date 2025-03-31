using AutoMapper;
using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Application.UseCases.UserUsecases
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IEmailConfirmationService _emailConfirmationService;

        public UpdateUserHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITokenService tokenService,
            IEmailConfirmationService emailConfirmationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
            _emailConfirmationService = emailConfirmationService;
        }

        public async Task<Unit> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
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
            var oldEmail = user.Email;
            _mapper.Map(request, user);
            if (!string.Equals(oldEmail, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.EmailVerifiedAt = null;
                var newConfirmationToken = await _emailConfirmationService.GenerateEmailConfirmationTokenAsync(user.Id);
                user.EmailConfirmationToken = newConfirmationToken;
                await _emailConfirmationService.SendConfirmationEmailAsync(user.Id);
            }

            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
