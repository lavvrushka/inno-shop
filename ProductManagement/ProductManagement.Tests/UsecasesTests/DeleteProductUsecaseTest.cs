using MediatR;
using Moq;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.UseCases.ProductUsecases;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Interfaces.IServices;
using ProductManagement.Domain.Models;

namespace ProductManagement.Tests.UsecasesTests
{
    public class DeleteProductHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly DeleteProductHandler _handler;

        public DeleteProductHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _tokenServiceMock = new Mock<ITokenService>();
            _handler = new DeleteProductHandler(_unitOfWorkMock.Object, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var deleteProductRequest = new DeleteProductRequest(productId);
            var userId = Guid.NewGuid();
            var product = new Product { Id = productId, UserId = userId };
            var token = "valid_token";

            _tokenServiceMock.Setup(t => t.ExtractTokenFromHeader()).Returns(token);
            _tokenServiceMock.Setup(t => t.ExtractUserIdFromToken(token)).Returns(userId);
            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(productId)).ReturnsAsync(product);
            _unitOfWorkMock.Setup(u => u.Products.DeleteAsync(product)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(deleteProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);
            _tokenServiceMock.Verify(t => t.ExtractTokenFromHeader(), Times.Once);
            _tokenServiceMock.Verify(t => t.ExtractUserIdFromToken(token), Times.Once);
            _unitOfWorkMock.Verify(u => u.Products.GetByIdAsync(productId), Times.Once);
            _unitOfWorkMock.Verify(u => u.Products.DeleteAsync(product), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
