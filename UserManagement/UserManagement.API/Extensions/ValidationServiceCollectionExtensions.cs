using FluentValidation;
using UserManagement.Application.Common.Validation.User;

namespace UserManagement.API.Extensions
{
    public static class ValidationServiceCollectionExtensions
    {
        public static IServiceCollection AddValidationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<UserLoginRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UserRegisterRequestValidator>();
            return services;
        }
    }
}
