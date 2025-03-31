using Moq;
using AutoMapper;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Domain.Models;
using ProductManagement.Domain.Interfaces.IRepositories;

namespace ProductManagement.Tests.UsecasesTests
{
    public class GetProductByIdHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetProductByIdHandler _handler;

        public GetProductByIdHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetProductByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProductResponse_WhenProductIsFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productEntity = new Product
            {
                Id = productId,
                Name = "Product 1",
                Description = "Description 1",
                Price = 100.00m,
                Quantity = 50,
                IsAvailable = true,
                ImageId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var productResponse = new ProductResponse(
                productEntity.Id,
                productEntity.Name,
                productEntity.Description,
                productEntity.Price,
                productEntity.IsAvailable,
                productEntity.Quantity,
                productEntity.ImageId ?? Guid.Empty, 
                DateTime.UtcNow,
                "ImageData",
                "ImageType",
                productEntity.ImageId ?? Guid.Empty 
            );

            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(productId)).ReturnsAsync(productEntity);
            _mapperMock.Setup(m => m.Map<ProductResponse>(productEntity)).Returns(productResponse);

            // Act
            var result = await _handler.Handle(new GetProductByIdRequest(productId), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productEntity.Name, result.Name);
            Assert.Equal(productEntity.Description, result.Description);
            Assert.Equal(productEntity.Price, result.Price);
            Assert.Equal(productEntity.Quantity, result.Quantity);
            Assert.Equal(productEntity.IsAvailable, result.IsAvailable);
        }
    }
}
