using AutoMapper;
using MediatR;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Models;


namespace ProductManagement.Application.UseCases.ProductUsecases
{
    public class GetProductByPageHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetProductsByPageAsyncRequest, Pagination<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        public async Task<Pagination<ProductResponse>> Handle(GetProductsByPageAsyncRequest request, CancellationToken cancellationToken)
        {
            var pageSettings = _mapper.Map<PageSettings>(request);
            var activeProducts = await _unitOfWork.Products.GetActiveProductsAsync();

            if (activeProducts == null || !activeProducts.Any())
            {
                throw new Exception("No products found.");
            }
            var totalCount = await _unitOfWork.Products.GetProductCountAsync();
            var productResponse = _mapper.Map<List<ProductResponse>>(activeProducts);
            return new Pagination<ProductResponse>(productResponse, totalCount, pageSettings);
        }
    }
}
