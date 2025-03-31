using AutoMapper;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Domain.Models;

namespace ProductManagement.Application.Common.Mappings
{
    public class PageSettingsProfile : Profile
    {
        public PageSettingsProfile()
        {
            CreateMap<GetProductsByPageAsyncRequest, PageSettings>()
                .ForMember(dest => dest.PageIndex, opt => opt.MapFrom(src => src.PageIndex))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize));
        }
    }
}
