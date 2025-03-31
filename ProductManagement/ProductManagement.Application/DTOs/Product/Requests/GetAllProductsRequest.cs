using MediatR;
using ProductManagement.Application.DTOs.Product.Responses;

namespace ProductManagement.Application.DTOs.Product.Requests
{
    public record GetAllProductsRequest : IRequest<List<ProductResponse>>;
}
