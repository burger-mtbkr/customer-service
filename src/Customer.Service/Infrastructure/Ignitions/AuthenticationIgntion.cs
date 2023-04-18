using Customer.Service.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Customer.Service.Ignitions
{
    public static class AuthenticationIgnition
    {
        public static void ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                var tokenConfig = builder.Configuration.GetSection("TokenSettings")
               .Get<TokenSettings>();

                cfg.RequireHttpsMetadata = Convert.ToBoolean(tokenConfig!.RequireHttpsMetadata);
                cfg.SaveToken = Convert.ToBoolean(tokenConfig!.SaveToken);
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = Convert.ToBoolean(tokenConfig!.ValidateIssuer),
                    ValidIssuer = tokenConfig!.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig!.Key)),

                    ValidateAudience = Convert.ToBoolean(tokenConfig.ValidateAudience),
                    ValidAudience = tokenConfig.Audience,

                    ValidateLifetime = Convert.ToBoolean(tokenConfig.ValidateLifetime),
                    RequireExpirationTime = Convert.ToBoolean(tokenConfig.RequireExpirationTime),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
    }
}
