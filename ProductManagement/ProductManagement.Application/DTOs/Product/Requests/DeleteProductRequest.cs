using MediatR;


namespace ProductManagement.Application.DTOs.Product.Requests
{
    public record DeleteProductRequest(Guid ProductId) : IRequest<Unit>;
}
