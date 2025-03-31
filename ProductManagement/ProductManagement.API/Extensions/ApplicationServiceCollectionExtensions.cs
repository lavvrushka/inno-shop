using ProductManagement.Application.UseCases.ProductUsecases;

namespace ProductManagement.API.Extensions
{
    public static class ApplicationCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<CreateProductHandler>();
            services.AddScoped<DeleteProductHandler>();
            services.AddScoped<GetProductByIdHandler>();
            services.AddScoped<GetProductsByUserHandler>();
            services.AddScoped<SearchProductsHandler>();
            services.AddScoped<UpdateProductHandler>();
            return services;
        }
    }
}
