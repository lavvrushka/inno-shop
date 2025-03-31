using Moq;
using MediatR;
using AutoMapper;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Application.UseCases.UserUsecases;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Models;
using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UserManagement.Tests.UseCases.UserUsecases
{
    public class DeleteUserHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IConnectionService> _mockConnectionService;
        private readonly DeleteUserHandler _handler;

        public DeleteUserHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockConnectionService = new Mock<IConnectionService>();
            _handler = new DeleteUserHandler(_mockUnitOfWork.Object, _mockMapper.Object, _mockConnectionService.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
            Assert.Equal("request", exception.ParamName);
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            // Arrange
            var request = new DeleteUserRequest(Guid.NewGuid());
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("Пользователь не найден.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldDeleteUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, FirstName = "John", LastName = "Doe" };

          
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
            _mockUnitOfWork.Setup(u => u.Users.DeleteAsync(It.IsAny<User>())).Returns(Task.CompletedTask);  

            // Act
            var result = await _handler.Handle(new DeleteUserRequest(userId), CancellationToken.None);

            // Assert
            _mockUnitOfWork.Verify(u => u.Users.DeleteAsync(It.Is<User>(u => u.Id == userId)), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);  
        }

    }
}
