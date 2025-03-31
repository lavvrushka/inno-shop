using MediatR;


namespace ProductManagement.Application.DTOs.Product.Requests
{
    public record HideProductsByUserRequest(Guid UserId) : IRequest<bool>;

}
