using MediatR;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Domain.Interfaces.IRepositories;


namespace ProductManagement.Application.UseCases.ProductUsecases
{
    public class DeleteAllProductsByUserHandler : IRequestHandler<DeleteAllProductsByUserRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAllProductsByUserHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteAllProductsByUserRequest request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.Products.GetByUserIdAsync(request.UserId);

            if (products != null)
            {
                _unitOfWork.Products.DeleteRange(products);
               
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
