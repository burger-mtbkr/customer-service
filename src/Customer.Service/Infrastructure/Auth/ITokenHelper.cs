using Customer.Service.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Customer.Service.Infrastructure.Auth
{
    public interface ITokenHelper
    {
        string CreateJwtToken(IConfiguration configuration, UserModel user);
        JwtSecurityToken ReadJwtToken(string tokenString);
        bool IsActive(string tokenString);
        string GetTokenUserId(string accessToken);
        T GetTokenPayload<T>(string tokenString, string key) where T : class;
        string GetBearerToken(HttpRequest request);
    }
}
