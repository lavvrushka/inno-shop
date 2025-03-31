using MediatR;
using ProductManagement.Application.DTOs.Product.Responses;


namespace ProductManagement.Application.DTOs.Product.Requests
{
    public record SearchProductsRequest(string SearchTerm) : IRequest<IEnumerable<ProductResponse>>;
}
