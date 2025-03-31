using Moq;
using AutoMapper;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Application.DTOs.User.Responses;
using UserManagement.Application.UseCases.UserUsecases;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Models;

namespace UserManagement.Tests.UseCases.UserUsecases
{
    public class GetAllUsersHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetAllUsersHandler _handler;

        public GetAllUsersHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetAllUsersHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenNoUsersFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Users.GetAllAsync()).ReturnsAsync(new List<User>());

            var request = new GetAllUsersRequest();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("No users found.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnUserList_WhenUsersAreFound()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
                new User { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" }
            };
            var userResponses = new List<UserResponse>
            {
                new UserResponse { Id = users[0].Id, FirstName = users[0].FirstName, LastName = users[0].LastName, Email = users[0].Email },
                new UserResponse { Id = users[1].Id, FirstName = users[1].FirstName, LastName = users[1].LastName, Email = users[1].Email }
            };

            _mockUnitOfWork.Setup(u => u.Users.GetAllAsync()).ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<List<UserResponse>>(users)).Returns(userResponses);

            var request = new GetAllUsersRequest();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("John", result[0].FirstName);
            Assert.Equal("Jane", result[1].FirstName);
        }
    }
}
