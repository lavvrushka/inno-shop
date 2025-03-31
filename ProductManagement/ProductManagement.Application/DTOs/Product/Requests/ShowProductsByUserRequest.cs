using MediatR;


namespace ProductManagement.Application.DTOs.Product.Requests
{
    public record ShowProductsByUserRequest(Guid UserId) : IRequest<bool>;
}
