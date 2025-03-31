using MediatR;
using ProductManagement.Application.DTOs.Product.Responses;

namespace ProductManagement.Application.DTOs.Product.Requests
{
    public record UpdateProductRequest(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        bool IsAvailable,
        int Quantity,
        Guid ImageId,
        string ImageData,
        string ImageType
    ) : IRequest<ProductResponse>;
}
