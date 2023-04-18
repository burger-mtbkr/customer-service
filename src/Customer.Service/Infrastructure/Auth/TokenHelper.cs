using Customer.Service.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Customer.Service.Infrastructure.Auth
{
    public class TokenHelper: ITokenHelper
    {
        public const string USER_ID_PAYLOAD_KEY = "USER_ID";
        public const string USER_NAME_PAYLOAD_KEY = "USER_NAME";
        public const string USER_EMAIL_PAYLOAD_KEY = "USER_EMAIL";

        /// <summary>
        /// Gets the JwtSecurityToken
        /// </summary>
        /// <returns></returns>
        public string CreateJwtToken(IConfiguration configuration, UserModel user)
        {
            var tokenConfig = configuration.GetSection("TokenSettings")
                 .Get<TokenSettings>();

            var expiryInHours = Convert.ToDouble(tokenConfig!.ExpiryHours);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(expiryInHours);

            var jwtToken = new JwtSecurityToken(
           tokenConfig.Issuer,
           tokenConfig.Audience,
            null,
            notBefore: DateTime.UtcNow.AddDays(-7),
            expires: expires,
            signingCredentials: creds);

            jwtToken.Payload[USER_ID_PAYLOAD_KEY] = user.Id;
            jwtToken.Payload[USER_EMAIL_PAYLOAD_KEY] = user.Email;
            jwtToken.Payload[USER_NAME_PAYLOAD_KEY] = $"{user.FirstName} {user.LastName}";

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        /// <summary>
        /// Gets the JwtSecurityToken from the encoded accessToken
        /// </summary>
        /// <returns></returns>
        public JwtSecurityToken ReadJwtToken(string tokenString)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
        }

        /// <summary>
        /// Checks accessToken expiry date
        /// </summary>		
        /// <param name="tokenString"></param>
        /// <returns></returns>
        public bool IsActive(string tokenString)
        {
            try
            {
                var jwt = ReadJwtToken(tokenString);
                if(jwt == null)
                {
                    return false;
                }

                if(DateTime.UtcNow < jwt.ValidTo)
                {
                    return true;
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Get User Id out of accessToken
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public string GetTokenUserId(string accessToken)
        {
            return GetTokenPayload<string>(accessToken, USER_ID_PAYLOAD_KEY);
        }

        /// <summary>
        /// Get Token Payload of Type T
        /// </summary>
        /// <param name="tokenString"></param>
        /// <param name="key"></param>
        public T GetTokenPayload<T>(string tokenString, string key) where T : class
        {
            var jwt = ReadJwtToken(tokenString);

            if(jwt.Payload.ContainsKey(key))
            {
                return (T)jwt.Payload[key];
            }
            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetBearerToken(HttpRequest request)
        {
            string authorization = request.Headers["Authorization"]!;

            // If no authorization header found, nothing to process further 
            if(!string.IsNullOrEmpty(authorization))
            {
                if(authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    return authorization.Substring("Bearer ".Length).Trim();
                }
            }
            return null;
        }
    }
}
