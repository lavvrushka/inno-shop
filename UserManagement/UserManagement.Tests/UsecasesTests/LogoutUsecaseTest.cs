using Moq;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Application.UseCases.AuthUsecases;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Models;
using MediatR;

namespace UserManagement.Tests.UseCases.AuthUsecases
{
    public class LogoutHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly LogoutHandler _handler;

        public LogoutHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTokenService = new Mock<ITokenService>();
            _handler = new LogoutHandler(_mockUnitOfWork.Object, _mockTokenService.Object);
        }

        [Fact]
        public async Task Handle_ShouldRevokeRefreshToken_WhenTokenIsValid()
        {
            // Arrange
            var refreshToken = new RefreshToken
            {
                Token = "valid-refresh-token",
                UserId = Guid.NewGuid()
            };

            var request = new UserLogoutRequest();

            _mockTokenService.Setup(t => t.ExtractTokenFromHeader()).Returns("valid-refresh-token");
            _mockUnitOfWork.Setup(u => u.RefreshTokens.GetByTokenAsync("valid-refresh-token")).ReturnsAsync(refreshToken);
            _mockTokenService.Setup(t => t.RevokeRefreshTokenAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);
            _mockTokenService.Verify(t => t.RevokeRefreshTokenAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenTokenIsMissing()
        {
            // Arrange
            var request = new UserLogoutRequest(); 

            _mockTokenService.Setup(t => t.ExtractTokenFromHeader()).Returns<string>(null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("Token is missing.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldNotRevokeToken_WhenRefreshTokenNotFound()
        {
            // Arrange
            var request = new UserLogoutRequest(); 

            _mockTokenService.Setup(t => t.ExtractTokenFromHeader()).Returns("invalid-refresh-token");
            _mockUnitOfWork.Setup(u => u.RefreshTokens.GetByTokenAsync("invalid-refresh-token")).ReturnsAsync((RefreshToken)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);
            _mockTokenService.Verify(t => t.RevokeRefreshTokenAsync(), Times.Never);
        }
    }
}
