using AutoMapper;
using MediatR;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Interfaces.IServices;

namespace ProductManagement.Application.UseCases.ProductUsecases
{
    public class GetProductsByUserHandler : IRequestHandler<GetProductsByUserRequest, IEnumerable<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public GetProductsByUserHandler(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponse>> Handle(GetProductsByUserRequest request, CancellationToken cancellationToken)
        {
            var token = _tokenService.ExtractTokenFromHeader();
            if (token == null)
            {
                throw new UnauthorizedAccessException("Authorization token is missing.");
            }

            var userId = _tokenService.ExtractUserIdFromToken(token)
                         ?? throw new UnauthorizedAccessException("Invalid authorization token.");

            var products = await _unitOfWork.Products.GetByUserIdAsync(userId);

            if (products == null || !products.Any())
            {
                return Enumerable.Empty<ProductResponse>();
            }

            var response = _mapper.Map<IEnumerable<ProductResponse>>(products);
            return response;
        }
    }
}
