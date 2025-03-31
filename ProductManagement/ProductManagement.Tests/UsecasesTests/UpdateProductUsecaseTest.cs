using AutoMapper;
using Moq;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.UseCases.ProductUsecases;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Interfaces.IServices;
using ProductManagement.Domain.Models;

namespace ProductManagement.Tests.UsecasesTests
{
    public class UpdateProductHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UpdateProductHandler _handler;

        public UpdateProductHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _tokenServiceMock = new Mock<ITokenService>();
            _mapperMock = new Mock<IMapper>();
            _handler = new UpdateProductHandler(_unitOfWorkMock.Object, _tokenServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedException_WhenTokenIsMissing()
        {
            // Arrange
            var updateRequest = new UpdateProductRequest(
                Guid.NewGuid(),
                "Updated Product",
                "Updated Description",
                100, 
                true,  
                10, 
                Guid.NewGuid(),
                "Updated ImageData",
                "imageType"
            );

            _tokenServiceMock.Setup(t => t.ExtractTokenFromHeader()).Returns<string>(null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(updateRequest, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedException_WhenTokenIsInvalid()
        {
            // Arrange
            var updateRequest = new UpdateProductRequest(
                Guid.NewGuid(),
                "Updated Product",
                "Updated Description",
                100,
                true,  
                10, 
                Guid.NewGuid(),
                "Updated ImageData",
                "imageType"
            );

            _tokenServiceMock.Setup(t => t.ExtractTokenFromHeader()).Returns("invalid-token");
            _tokenServiceMock.Setup(t => t.ExtractUserIdFromToken(It.IsAny<string>())).Returns<string>(null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(updateRequest, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenProductNotFound()
        {
            // Arrange
            var updateRequest = new UpdateProductRequest(
                Guid.NewGuid(),
                "Updated Product",
                "Updated Description",
                100,
                true,  
                10, 
                Guid.NewGuid(),
                "Updated ImageData",
                "imageType"
            );

            _tokenServiceMock.Setup(t => t.ExtractTokenFromHeader()).Returns("valid-token");
            _tokenServiceMock.Setup(t => t.ExtractUserIdFromToken(It.IsAny<string>())).Returns(Guid.NewGuid());
            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(updateRequest, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserNotAuthorizedToUpdateProduct()
        {
            // Arrange
            var updateRequest = new UpdateProductRequest(
                Guid.NewGuid(),
                "Updated Product",
                "Updated Description",
                100,
                true,  
                10, 
                Guid.NewGuid(),
                "Updated ImageData",
                "imageType"
            );

            var productEntity = new Product { Id = updateRequest.Id, UserId = Guid.NewGuid() };

            _tokenServiceMock.Setup(t => t.ExtractTokenFromHeader()).Returns("valid-token");
            _tokenServiceMock.Setup(t => t.ExtractUserIdFromToken(It.IsAny<string>())).Returns(Guid.NewGuid());
            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(productEntity);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(updateRequest, CancellationToken.None));
        }

    }
}
