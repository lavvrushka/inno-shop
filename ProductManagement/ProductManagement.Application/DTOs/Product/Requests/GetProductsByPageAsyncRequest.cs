using MediatR;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Domain.Models;


namespace ProductManagement.Application.DTOs.Product.Requests
{
    public record GetProductsByPageAsyncRequest(
            int PageIndex,
            int PageSize
        ) : IRequest<Pagination<ProductResponse>>;
}
