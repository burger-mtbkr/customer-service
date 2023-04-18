using Customer.Service.Exceptions;
using System.Net;

namespace Customer.Service.MIddleware
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<ErrorMiddleware> logger)
        {
            try
            {
                await next(context);
            }
            catch(CustomerNotFoundException e)
            {
                logger.LogError(e.Message, e);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            catch(LeadNotFoundException e)
            {
                logger.LogError(e.Message, e);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            catch(UserNotFoundException e)
            {
                logger.LogError(e.Message, e);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            catch(InvalidPasswordException e)
            {
                logger.LogError(e.Message, e);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            catch(EmailAlreadyInUseException e)
            {
                logger.LogError(e.Message, e);
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            }
            catch(UnauthorizedAccessException e)
            {
                logger.LogError(e.Message, e);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            catch(Exception e)
            {
                logger.LogError(e.Message, e);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
