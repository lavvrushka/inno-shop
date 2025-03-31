using Moq;
using AutoMapper;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Application.UseCases.ProductUsecases;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Models;

namespace ProductManagement.Tests.UsecasesTests
{
    public class GetAllProductsHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllProductsHandler _handler;

        public GetAllProductsHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAllProductsHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProductResponses_WhenProductsExist()
        {
            // Arrange
            var productEntities = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product 1", Description = "Desc 1", Price = 10.0m, Quantity = 5, IsAvailable = true },
                new Product { Id = Guid.NewGuid(), Name = "Product 2", Description = "Desc 2", Price = 20.0m, Quantity = 10, IsAvailable = true }
            };

            var productResponses = new List<ProductResponse>
{
    new ProductResponse(
        productEntities[0].Id,
        productEntities[0].Name,
        productEntities[0].Description,
        productEntities[0].Price,
        productEntities[0].IsAvailable,
        productEntities[0].Quantity,
        productEntities[0].ImageId ?? Guid.NewGuid(), 
        DateTime.UtcNow,
        "imageDataSample",
        "imageTypeSample",
        Guid.NewGuid()
    ),
    new ProductResponse(
        productEntities[1].Id,
        productEntities[1].Name,
        productEntities[1].Description,
        productEntities[1].Price,
        productEntities[1].IsAvailable,
        productEntities[1].Quantity,
        productEntities[1].ImageId ?? Guid.NewGuid(),
        DateTime.UtcNow,
        "imageDataSample",
        "imageTypeSample",
        Guid.NewGuid()
    )
};

            _unitOfWorkMock.Setup(u => u.Products.GetActiveProductsAsync()).ReturnsAsync(productEntities);
            _mapperMock.Setup(m => m.Map<List<ProductResponse>>(productEntities)).Returns(productResponses);

            // Act
            var result = await _handler.Handle(new GetAllProductsRequest(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Product 1", result[0].Name);
            Assert.Equal("Product 2", result[1].Name);

            _unitOfWorkMock.Verify(u => u.Products.GetActiveProductsAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<List<ProductResponse>>(productEntities), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenNoProductsFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Products.GetActiveProductsAsync()).ReturnsAsync(new List<Product>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(new GetAllProductsRequest(), CancellationToken.None));
            Assert.Equal("No products found.", exception.Message);

            _unitOfWorkMock.Verify(u => u.Products.GetActiveProductsAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<List<ProductResponse>>(It.IsAny<List<Product>>()), Times.Never);
        }
    }
}
