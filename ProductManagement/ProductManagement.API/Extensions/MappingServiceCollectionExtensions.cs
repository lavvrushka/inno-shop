using ProductManagement.Application.Common.Mappings;

namespace ProductManagement.API.Extensions
{
    public static class MappingServiceCollectionExtensions
    {
        public static IServiceCollection AddMappingProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ProductProfile).Assembly);
            return services;
        }
    }
}
