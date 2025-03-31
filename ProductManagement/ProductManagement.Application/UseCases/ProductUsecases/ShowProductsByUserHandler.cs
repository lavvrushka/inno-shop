using MediatR;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Domain.Interfaces.IRepositories;

namespace ProductManagement.Application.UseCases.ProductUsecases
{
    public class ShowProductsByUserHandler : IRequestHandler<ShowProductsByUserRequest, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ShowProductsByUserHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ShowProductsByUserRequest request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetByUserIdAsync(request.UserId);

            if (products == null || !products.Any())
            {
                return false;
            }
            await _unitOfWork.Products.RestoreProductsByOwnerIdAsync(request.UserId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
