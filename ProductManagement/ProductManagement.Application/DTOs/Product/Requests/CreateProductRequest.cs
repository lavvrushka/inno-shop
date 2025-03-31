using MediatR;
using ProductManagement.Application.DTOs.Product.Responses;

namespace ProductManagement.Application.DTOs.Product.Requests
{
    public record CreateProductRequest(
         string Name,
         string Description,
         decimal Price,
         int Quantity,
         bool IsAvailable,
         string ImageData,
         string ImageType
     ) : IRequest<ProductResponse>;
}
