using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Service.Controllers
{
    [DisableCors]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController: ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get() => Ok();
    }
}
