using Moq;
using AutoMapper;
using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Application.DTOs.User.Responses;
using UserManagement.Application.UseCases.AuthUsecases;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Models;
using Xunit;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace UserManagement.Tests.UseCases.AuthUsecases
{
    public class RegisterUserHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IHashPassword> _mockHashPassword;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IEmailConfirmationService> _mockEmailConfirmationService;
        private readonly RegisterUserHandler _handler;

        public RegisterUserHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTokenService = new Mock<ITokenService>();
            _mockHashPassword = new Mock<IHashPassword>();
            _mockMapper = new Mock<IMapper>();
            _mockEmailConfirmationService = new Mock<IEmailConfirmationService>();

            _handler = new RegisterUserHandler(
                _mockUnitOfWork.Object,
                _mockTokenService.Object,
                _mockHashPassword.Object,
                _mockMapper.Object,
                _mockEmailConfirmationService.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserAlreadyExists()
        {
            // Arrange
            var request = new UserRegisterRequest(
                "newuser@example.com",  
                "John",                  
                "Doe",                   
                "Password123",            
                DateTime.Now.AddYears(-25) 
            );

            var existingUser = new User
            {
                Email = request.Email
            };

            _mockUnitOfWork.Setup(u => u.Users.GetByEmailAsync(request.Email)).ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("User already exists.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldCreateNewUserAndReturnResponse_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new UserRegisterRequest(
                "newuser@example.com",   
                "John",                   
                "Doe",                    
                "Password123",           
                DateTime.Now.AddYears(-25) 
            );

            var newUser = new User
            {
                FirstName = request.Firstname,
                LastName = request.Lastname,
                Email = request.Email,
                Password = "hashedPassword",
                BirthDate = request.BirthDate,
                RoleId = Guid.NewGuid() 
            };

            var role = new Role { Id = Guid.NewGuid(), Name = "User" };

            _mockUnitOfWork.Setup(u => u.Users.GetByEmailAsync(request.Email)).ReturnsAsync((User)null); 
            _mockUnitOfWork.Setup(u => u.Roles.GetByNameAsync("User")).ReturnsAsync(role);
            _mockHashPassword.Setup(h => h.Hash(request.Password)).Returns("hashedPassword");
            _mockUnitOfWork.Setup(u => u.Users.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockEmailConfirmationService.Setup(e => e.GenerateEmailConfirmationTokenAsync(It.IsAny<Guid>())).ReturnsAsync("confirmationToken");
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mockEmailConfirmationService.Setup(e => e.SendConfirmationEmailAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

            var expectedResponse = new UserRegisterResponse("accessToken", "refreshToken");

            _mockTokenService.Setup(t => t.GenerateAccessToken(It.IsAny<User>())).ReturnsAsync("accessToken");
            _mockTokenService.Setup(t => t.GenerateRefreshTokenAsync(It.IsAny<Guid>())).ReturnsAsync("refreshToken");
            _mockMapper.Setup(m => m.Map<UserRegisterResponse>(It.IsAny<(string, string)>())).Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.AccessToken, result.AccessToken);
            Assert.Equal(expectedResponse.RefreshToken, result.RefreshToken);
            _mockUnitOfWork.Verify(u => u.Users.AddAsync(It.IsAny<User>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            _mockEmailConfirmationService.Verify(e => e.SendConfirmationEmailAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
