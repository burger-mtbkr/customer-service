using Customer.Service.Repositories;

namespace Customer.Service.Ignition
{
    public static class RepositoryIgnition
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }
    }
}
