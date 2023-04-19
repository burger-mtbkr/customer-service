using Customer.Service.Models;
using Customer.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Service.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController: ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        /// <summary>
        /// Validated if current token is still active
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<Session>), StatusCodes.Status200OK)]
        public ActionResult<bool> IsTokenActive()
        {
            return Ok(_sessionService.IsTokenActive());
        }
    }
}
