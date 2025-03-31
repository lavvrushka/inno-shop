using MediatR;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Interfaces.IServices;

namespace ProductManagement.Application.UseCases.ProductUsecases
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public DeleteProductHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<Unit> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
        {
            var token = _tokenService.ExtractTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Token is missing.");
            }

            var userId = _tokenService.ExtractUserIdFromToken(token);
            if (!userId.HasValue)
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                throw new Exception($"Product with ID '{request.ProductId}' not found.");
            }

            if (product.UserId != userId.Value)
            {
                throw new Exception("You are not allowed to delete this product.");
            }

            await _unitOfWork.Products.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
