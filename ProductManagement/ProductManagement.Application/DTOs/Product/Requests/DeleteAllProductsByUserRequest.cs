
using MediatR;

namespace ProductManagement.Application.DTOs.Product.Requests
{
    public record DeleteAllProductsByUserRequest(Guid UserId) : IRequest<bool>;
}
