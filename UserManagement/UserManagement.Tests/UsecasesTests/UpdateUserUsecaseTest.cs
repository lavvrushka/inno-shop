using Moq;
using AutoMapper;
using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Application.UseCases.UserUsecases;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Models;


namespace UserManagement.Tests.UseCases.UserUsecases
{
    public class UpdateUserHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IEmailConfirmationService> _mockEmailConfirmationService;
        private readonly UpdateUserHandler _handler;

        public UpdateUserHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockTokenService = new Mock<ITokenService>();
            _mockEmailConfirmationService = new Mock<IEmailConfirmationService>();

            _handler = new UpdateUserHandler(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockTokenService.Object,
                _mockEmailConfirmationService.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenTokenIsMissing()
        {
            // Arrange
            _mockTokenService.Setup(t => t.ExtractTokenFromHeader()).Returns<string>(null);

            var request = new UpdateUserRequest("John", "Doe", "newemail@example.com");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("Token is missing.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var token = "validToken";
            var userId = Guid.NewGuid();
            _mockTokenService.Setup(t => t.ExtractTokenFromHeader()).Returns(token);
            _mockTokenService.Setup(t => t.ExtractUserIdFromToken(token)).Returns(userId);
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var request = new UpdateUserRequest("John", "Doe", "newemail@example.com");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("User not found.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldUpdateUserAndSendEmailConfirmation_WhenEmailIsChanged()
        {
            // Arrange
            var token = "validToken";
            var userId = Guid.NewGuid();
            var oldUser = new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "oldemail@example.com"
            };

            var request = new UpdateUserRequest("John", "Doe", "newemail@example.com");

            _mockTokenService.Setup(t => t.ExtractTokenFromHeader()).Returns(token);
            _mockTokenService.Setup(t => t.ExtractUserIdFromToken(token)).Returns(userId);
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId)).ReturnsAsync(oldUser);
            _mockMapper.Setup(m => m.Map(It.IsAny<UpdateUserRequest>(), It.IsAny<User>())).Callback<UpdateUserRequest, User>((req, u) =>
            {
                u.FirstName = req.FirstName;
                u.LastName = req.LastName;
                u.Email = req.Email;
            });

            _mockEmailConfirmationService.Setup(e => e.GenerateEmailConfirmationTokenAsync(userId)).ReturnsAsync("newConfirmationToken");
            _mockEmailConfirmationService.Setup(e => e.SendConfirmationEmailAsync(userId)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            _mockEmailConfirmationService.Verify(e => e.GenerateEmailConfirmationTokenAsync(userId), Times.Once);
            _mockEmailConfirmationService.Verify(e => e.SendConfirmationEmailAsync(userId), Times.Once);
            Assert.Null(oldUser.EmailVerifiedAt);
        }

        [Fact]
        public async Task Handle_ShouldUpdateUser_WhenEmailIsNotChanged()
        {
            // Arrange
            var token = "validToken";
            var userId = Guid.NewGuid();
            var oldUser = new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "oldemail@example.com"
            };

            var request = new UpdateUserRequest("John", "Doe", "oldemail@example.com");

            _mockTokenService.Setup(t => t.ExtractTokenFromHeader()).Returns(token);
            _mockTokenService.Setup(t => t.ExtractUserIdFromToken(token)).Returns(userId);
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId)).ReturnsAsync(oldUser);
            _mockMapper.Setup(m => m.Map(It.IsAny<UpdateUserRequest>(), It.IsAny<User>())).Callback<UpdateUserRequest, User>((req, u) =>
            {
                u.FirstName = req.FirstName;
                u.LastName = req.LastName;
                u.Email = req.Email;
            });

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            _mockEmailConfirmationService.Verify(e => e.GenerateEmailConfirmationTokenAsync(It.IsAny<Guid>()), Times.Never);
            _mockEmailConfirmationService.Verify(e => e.SendConfirmationEmailAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
