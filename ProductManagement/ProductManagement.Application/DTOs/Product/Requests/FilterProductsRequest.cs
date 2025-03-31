using MediatR;
using ProductManagement.Application.DTOs.Product.Responses;

namespace ProductManagement.Application.DTOs.Product.Requests
{
    public record FilterProductsRequest(bool? IsAvailable, decimal? MinPrice, decimal? MaxPrice)
       : IRequest<List<ProductResponse>>;
}