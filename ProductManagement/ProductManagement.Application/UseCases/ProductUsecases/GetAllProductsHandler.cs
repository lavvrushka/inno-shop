using AutoMapper;
using MediatR;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Domain.Interfaces.IRepositories;


namespace ProductManagement.Application.UseCases.ProductUsecases
{
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsRequest, List<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllProductsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ProductResponse>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            var activeProducts = await _unitOfWork.Products.GetActiveProductsAsync();

            if (activeProducts == null || !activeProducts.Any())
            {
                throw new Exception("No products found.");
            }

            var productResponse = _mapper.Map<List<ProductResponse>>(activeProducts);
            return productResponse;
        }
    }
}
