using UserManagement.API.Middlewares;

namespace UserManagement.API.Extensions
{
    public static class MiddlewareServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomMiddlewares(this IServiceCollection services)
        {
            services.AddTransient<GlobalExceptionMiddleware>();
            return services;
        }
    }
}
