using Customer.Service.Infrastructure.Auth;
using Customer.Service.Repositories;
using Customer.Service.Services;

namespace Customer.Service.Ignition
{
    public static class RepositoryIgnition
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
        }
    }
}
