using MediatR;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Domain.Interfaces.IRepositories;

namespace ProductManagement.Application.UseCases.ProductUsecases
{
    public class HideProductsByUserHandler : IRequestHandler<HideProductsByUserRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HideProductsByUserHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(HideProductsByUserRequest request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.Products.GetByUserIdAsync(request.UserId);

            if (products == null || !products.Any())
            {
                return false;
            }
            await _unitOfWork.Products.HideProductsByUserIdAsync(request.UserId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }

}
