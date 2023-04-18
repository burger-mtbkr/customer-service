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
        /// Returns all sessions
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Session>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Session>> Get()
        {
            var sessions = _sessionService.GetAll();
            return Ok(sessions);
        }

        /// <summary>
        /// Gets a single session by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Session), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UnauthorizedAccessException), StatusCodes.Status401Unauthorized)]
        public ActionResult<Session> Get(string id)
        {
            var sessions = _sessionService.GetSession(id);
            return Ok(sessions);
        }

        /// <summary>
        /// Delete a session by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ArgumentNullException), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            await _sessionService.DeleteSession(id);
            return NoContent();
        }

        /// <summary>
        /// deletes all sessions for current user
        /// </summary>
        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete()
        {
            await _sessionService.DeleteAllSessionForCurrentUser();
            return NoContent();
        }
    }
}
