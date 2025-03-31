using AutoMapper;
using MediatR;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Models;
using System.Linq.Expressions;

public class FilterProductsHandler : IRequestHandler<FilterProductsRequest, List<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public FilterProductsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ProductResponse>> Handle(FilterProductsRequest request, CancellationToken cancellationToken)
    {
        Expression<Func<Product, bool>> filter = p =>
            !p.IsDeleted &&
            (!request.IsAvailable.HasValue || p.IsAvailable == request.IsAvailable.Value) &&
            (!request.MinPrice.HasValue || p.Price >= request.MinPrice.Value) &&
            (!request.MaxPrice.HasValue || p.Price <= request.MaxPrice.Value);

        var products = await _unitOfWork.Products.GetFilteredAsync(filter);
        return _mapper.Map<List<ProductResponse>>(products);
    }
}