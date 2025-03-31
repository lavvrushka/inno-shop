using AutoMapper;
using Moq;
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
    public class LoginHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IHashPassword> _mockHashPassword;
        private readonly Mock<IMapper> _mockMapper;
        private readonly LoginHandler _handler;

        public LoginHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTokenService = new Mock<ITokenService>();
            _mockHashPassword = new Mock<IHashPassword>();
            _mockMapper = new Mock<IMapper>();

            _handler = new LoginHandler(
                _mockUnitOfWork.Object,
                _mockTokenService.Object,
                _mockHashPassword.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnAccessToken_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                Password = "hashedpassword123", 
                IsActive = true
            };

            var request = new UserLoginRequest("user@example.com", "correctpassword");

            _mockUnitOfWork.Setup(u => u.Users.GetByEmailAsync(request.Email)).ReturnsAsync(user);
            _mockHashPassword.Setup(h => h.VerifyPassword(user.Password, request.Password)).Returns(true);
            _mockUnitOfWork.Setup(u => u.Users.IsUserActiveAsync(user.Id)).ReturnsAsync(true);

            var accessToken = "accessToken123";
            var refreshToken = "refreshToken123";

            _mockTokenService.Setup(t => t.GenerateAccessToken(user)).ReturnsAsync(accessToken);
            _mockTokenService.Setup(t => t.GenerateRefreshTokenAsync(user.Id)).ReturnsAsync(refreshToken);

            // Map explicitly
            var expectedResponse = new UserLoginResponse(accessToken, refreshToken);
            _mockMapper.Setup(m => m.Map<UserLoginResponse>(It.IsAny<(string, string)>())).Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(accessToken, result.AccessToken);
            Assert.Equal(refreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                Password = "hashedpassword123", 
                IsActive = true
            };

            var request = new UserLoginRequest("user@example.com", "wrongpassword");

            _mockUnitOfWork.Setup(u => u.Users.GetByEmailAsync(request.Email)).ReturnsAsync(user);
            _mockHashPassword.Setup(h => h.VerifyPassword(user.Password, request.Password)).Returns(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("Invalid credentials.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIsInactive()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                Password = "hashedpassword123", 
                IsActive = false
            };

            var request = new UserLoginRequest("user@example.com", "correctpassword");

            _mockUnitOfWork.Setup(u => u.Users.GetByEmailAsync(request.Email)).ReturnsAsync(user);
            _mockHashPassword.Setup(h => h.VerifyPassword(user.Password, request.Password)).Returns(true);
            _mockUnitOfWork.Setup(u => u.Users.IsUserActiveAsync(user.Id)).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("User account is deactivated.", exception.Message);
        }
    }
}
