using Customer.Service.Models;

namespace Customer.Service.Services
{
    public interface ILoginService
    {
        Task<string?> Login(LoginRequest request);
        Task<bool> Logout();
    }
}
