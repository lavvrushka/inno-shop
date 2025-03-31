using AutoMapper;
using MediatR;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Domain.Interfaces.IRepositories;


namespace ProductManagement.Application.UseCases.ProductUsecases
{
    public class SearchProductsHandler : IRequestHandler<SearchProductsRequest, IEnumerable<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SearchProductsHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponse>> Handle(SearchProductsRequest request, CancellationToken cancellationToken)
        {
          
            var products = await _unitOfWork.Products.SearchProductsAsync(request.SearchTerm);

            if (products == null || !products.Any())
            {
                return Enumerable.Empty<ProductResponse>();
            }

            var response = _mapper.Map<IEnumerable<ProductResponse>>(products);
            return response;
        }
    }
}
