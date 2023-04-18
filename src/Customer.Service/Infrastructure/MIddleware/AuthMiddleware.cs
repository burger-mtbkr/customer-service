using Customer.Service.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Customer.Service.MIddleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate next;

        public AuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, RequestContext requestContext, ITokenHelper tokenHelper, ILogger<ErrorMiddleware> logger)
        {

            // Check if the request is unauthorized
            var endpoint = context.GetEndpoint();
            if(endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                await next(context);
                return;
            }

            var bearerToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if(string.IsNullOrEmpty(bearerToken))
            {
                logger.LogError("Token Is Null Or Empty ");
                throw new UnauthorizedAccessException(nameof(bearerToken));
            }

            if(!tokenHelper.IsActive(bearerToken)) throw new UnauthorizedAccessException("Session has expired");

            var userId = tokenHelper.GetTokenUserId(bearerToken);
            requestContext.UserId = userId;
            requestContext.Token = bearerToken;

            await next(context);

        }
    }
}
