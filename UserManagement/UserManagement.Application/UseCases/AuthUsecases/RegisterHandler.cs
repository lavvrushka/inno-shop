using AutoMapper;
using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Application.DTOs.User.Responses;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Models;

namespace UserManagement.Application.UseCases.AuthUsecases
{
    public class RegisterUserHandler : IRequestHandler<UserRegisterRequest, UserRegisterResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IHashPassword _hashPassword;
        private readonly IMapper _mapper;
        private readonly IEmailConfirmationService _emailConfirmationService;

        public RegisterUserHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IHashPassword hashPassword, IMapper mapper, IEmailConfirmationService emailConfirmationService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _hashPassword = hashPassword;
            _mapper = mapper;
            _emailConfirmationService = emailConfirmationService;
        }

        public async Task<UserRegisterResponse> Handle(UserRegisterRequest request, CancellationToken cancellationToken)
        {
            var existingProfile = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (existingProfile != null)
            {
                throw new InvalidOperationException("User already exists.");
            }
            var role = await _unitOfWork.Roles.GetByNameAsync("User");
            var hashedPassword = _hashPassword.Hash(request.Password);
            var newUser = new User
            {
                FirstName = request.Firstname,
                LastName = request.Lastname,
                Password = hashedPassword,
                BirthDate = request.BirthDate,
                Email = request.Email,
                RoleId = role.Id
            };
            await _unitOfWork.Users.AddAsync(newUser);
            newUser.EmailConfirmationToken = await _emailConfirmationService.GenerateEmailConfirmationTokenAsync(newUser.Id);
            await _unitOfWork.SaveChangesAsync();

            await _emailConfirmationService.SendConfirmationEmailAsync(newUser.Id);

            var accessToken = await _tokenService.GenerateAccessToken(newUser);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(newUser.Id);

            return _mapper.Map<UserRegisterResponse>((accessToken, refreshToken));
        }
    }
}
