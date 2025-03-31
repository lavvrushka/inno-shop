using AutoMapper;
using Moq;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Application.UseCases.ProductUsecases;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Interfaces.IServices;
using ProductManagement.Domain.Models;

namespace ProductManagement.Tests.UsecasesTests
{
    public class CreateProductHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IConnectionService> _connectionServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly CreateProductHandler _handler;

        public CreateProductHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _connectionServiceMock = new Mock<IConnectionService>();
            _mapperMock = new Mock<IMapper>();
            _tokenServiceMock = new Mock<ITokenService>();
            _handler = new CreateProductHandler(_connectionServiceMock.Object, _mapperMock.Object, _tokenServiceMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldMapAndAddProduct()
        {
            // Arrange
            var createProductRequest = new CreateProductRequest(
                "Sample Product",
                "Sample Description",
                99.99m,
                10,
                true,
                "imageData",
                "imageType"
            );

            var userId = Guid.NewGuid();
            var token = "sample_token";
            var productEntity = new Product { UserId = userId };
            var image = new Image();
            var imageId = Guid.NewGuid();
            var response = new ProductResponse();

            _tokenServiceMock.Setup(t => t.ExtractTokenFromHeader()).Returns(token);
            _tokenServiceMock.Setup(t => t.ExtractUserIdFromToken(token)).Returns(userId);
            _connectionServiceMock.Setup(c => c.UserExistsAndIsActiveAsync(userId)).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<Product>(It.IsAny<CreateProductRequest>())).Returns(productEntity);
            _mapperMock.Setup(m => m.Map<Image>(It.IsAny<CreateProductRequest>())).Returns(image);
            _unitOfWorkMock.Setup(u => u.Images.AddImageToProductAsync(It.IsAny<Image>())).ReturnsAsync(imageId);
            _unitOfWorkMock.Setup(u => u.Products.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<ProductResponse>(It.IsAny<Product>())).Returns(response);

            // Act
            var result = await _handler.Handle(createProductRequest, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(response, result);
            _tokenServiceMock.Verify(t => t.ExtractTokenFromHeader(), Times.Once);
            _tokenServiceMock.Verify(t => t.ExtractUserIdFromToken(token), Times.Once);
            _connectionServiceMock.Verify(c => c.UserExistsAndIsActiveAsync(userId), Times.Once);
            _mapperMock.Verify(m => m.Map<Product>(It.IsAny<CreateProductRequest>()), Times.Once);
            _mapperMock.Verify(m => m.Map<Image>(It.IsAny<CreateProductRequest>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Images.AddImageToProductAsync(It.IsAny<Image>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Products.AddAsync(It.IsAny<Product>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}