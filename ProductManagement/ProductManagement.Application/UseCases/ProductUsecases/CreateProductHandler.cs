using AutoMapper;
using MediatR;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Interfaces.IServices;
using ProductManagement.Domain.Models;

namespace ProductManagement.Application.UseCases.ProductUsecases
{
    public class CreateProductHandler : IRequestHandler<CreateProductRequest, ProductResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConnectionService _connectionService;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public CreateProductHandler(IConnectionService connectionService, IMapper mapper, ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _connectionService = connectionService;
            _mapper = mapper;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductResponse> Handle(CreateProductRequest request, CancellationToken cancellationToken)
        {
            var token = _tokenService.ExtractTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token is missing.");

            var userId = _tokenService.ExtractUserIdFromToken(token);
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("Invalid token.");

            var userExists = await _connectionService.UserExistsAndIsActiveAsync(userId.Value);
            if (!userExists)
                throw new UnauthorizedAccessException("User not found or inactive.");

            var product = _mapper.Map<Product>(request);
            product.UserId = userId.Value;
            var image = _mapper.Map<Image>(request);
            var imageId = await _unitOfWork.Images.AddImageToProductAsync(image);
            product.ImageId = imageId;
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductResponse>(product);
        }
    }

}
