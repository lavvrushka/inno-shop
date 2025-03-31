using AutoMapper;
using MediatR;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Application.Common.Exeptions;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Interfaces.IServices;

namespace ProductManagement.Application.UseCases.ProductUsecases
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductRequest, ProductResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public UpdateProductHandler(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<ProductResponse> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
        {
            var token = _tokenService.ExtractTokenFromHeader();
            if (token == null)
            {
                throw new UnauthorizedAccessException("Authorization token is missing.");
            }
            var userId = _tokenService.ExtractUserIdFromToken(token)
                         ?? throw new UnauthorizedAccessException("Invalid authorization token.");

            var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
            if (product == null)
            {
                throw new Exception("Product not found.");
            }
            if (product.UserId != userId)
            {
                throw new Exception("You are not allowed to update this product.");
            }
            var image = await _unitOfWork.Images.GetByIdAsync(request.ImageId);
            if (image == null)
            {
                throw new NotFoundException("Image", request.ImageId);
            }

            _mapper.Map(request, product);
            _mapper.Map(request, image);
            product.ImageId = image.Id;

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<ProductResponse>(product);
            return response;
        }
    }
}
