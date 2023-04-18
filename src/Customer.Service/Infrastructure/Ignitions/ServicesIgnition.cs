using Customer.Service.Infrastructure.Auth;
using Customer.Service.Services;
using Customer.Service.Tests.Services;

namespace Customer.Service.Ignition
{
    public static class ServiceIgnition
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<ITokenHelper, TokenHelper>();
            services.AddSingleton<IPasswordHash, PasswordHash>();

            services.AddScoped<RequestContext>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ILeadsService, LeadsService>();
        }
    }
}
