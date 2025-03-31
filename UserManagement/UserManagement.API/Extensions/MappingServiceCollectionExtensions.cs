using UserManagement.Application.Common.Mappings;

namespace UserManagement.API.Extensions
{
    public static class MappingServiceCollectionExtensions
    {
        public static IServiceCollection AddMappingProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserProfile).Assembly);
            return services;
        }
    }
}
