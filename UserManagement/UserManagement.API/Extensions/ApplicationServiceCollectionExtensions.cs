using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Application.UseCases.AuthUsecases;
using UserManagement.Application.UseCases.UserUsecases;

namespace UserManagement.API.Extensions
{
    public static class ApplicationCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(UserLoginRequest).Assembly);
            });

            services.AddScoped<LoginHandler>();
            services.AddScoped<LogoutHandler>();
            services.AddScoped<RefreshTokenHandler>();
            services.AddScoped<RegisterUserHandler>();
            services.AddScoped<ConfirmEmailHandler>();
            services.AddScoped<SendPasswordTokenHandler>();
            services.AddScoped<SetNewPasswordHandler>();
            services.AddScoped<DeactivateUserHandler>();
            services.AddScoped<CurrentUserHandler>();
            services.AddScoped<RecoverAccountHandler>();
            services.AddScoped<SendAccountTokenHandler>();
            services.AddScoped<UserStatusHandler>();
            return services;
        }
    }
}
