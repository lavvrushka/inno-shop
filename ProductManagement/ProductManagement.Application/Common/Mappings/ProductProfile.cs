using AutoMapper;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Domain.Models;
namespace ProductManagement.Application.Common.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductRequest, Product>();
            CreateMap<UpdateProductRequest, Product>()
             .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Product, ProductResponse>()
            .ForMember(dest => dest.ImageData, opt => opt.MapFrom(src => src.Image != null ? src.Image.ImageData : null))
            .ForMember(dest => dest.ImageType, opt => opt.MapFrom(src => src.Image != null ? src.Image.ImageType : null));


            CreateMap<Image, CreateProductRequest>().ReverseMap();
            CreateMap<Image, UpdateProductRequest>().ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ImageData, opt => opt.Condition(src => src.ImageData != null))
            .ForMember(dest => dest.ImageType, opt => opt.Condition(src => src.ImageType != null));

            CreateMap<DeleteProductRequest, Product>().ReverseMap()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id));


        }

    }
}
