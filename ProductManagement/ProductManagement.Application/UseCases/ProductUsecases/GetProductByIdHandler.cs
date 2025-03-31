using AutoMapper;
using MediatR;
using ProductManagement.Application.Common.Exeptions;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Models;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdRequest, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductResponse> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
        if (product == null || product.IsDeleted)
        {
            throw new NotFoundException(nameof(Product), request.ProductId);
        }

        var response = _mapper.Map<ProductResponse>(product);
        return response;
    }
}