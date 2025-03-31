using AutoMapper;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Application.DTOs.User.Responses;
using UserManagement.Domain.Models;

namespace UserManagement.Application.Common.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserLoginResponse>();
            CreateMap<User, UserLoginResponse>();
            CreateMap<User, UserLoginResponse>();
            CreateMap<UserRegisterRequest, User>();
            CreateMap<(string AccessToken, string RefreshToken), UserRegisterResponse>()
           .ConstructUsing(tuple => new UserRegisterResponse(tuple.AccessToken, tuple.RefreshToken));
            CreateMap<(string AccessToken, string RefreshToken), UserLoginResponse>()
            .ConstructUsing(tuple => new UserLoginResponse(tuple.AccessToken, tuple.RefreshToken));
            CreateMap<User, UserResponse>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<UserResponse, User>();
            CreateMap<(string AccessToken, string RefreshToken), UserTokenRespones>()
                  .ConstructUsing(tuple => new UserTokenRespones(tuple.AccessToken, tuple.RefreshToken));
            CreateMap<UpdateUserRequest, User>()
             .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
             .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
            CreateMap<DeleteUserRequest, User>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        }
    }
}
