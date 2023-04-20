using Customer.Service.Models;
using Customer.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Service.Controllers
{
    [DisableCors]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SignupController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;

        public SignupController(IUserService userService, ISessionService sessionService)
        {
            _userService = userService;
            _sessionService = sessionService;
        }

        /// <summary>
		/// Signup creates a new Login for an application
		/// </summary>
		/// <param name="model"></param>		
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> Post([FromBody] SignupRequest model)
        {
            if(model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Email or password has not been provoded");
            }

            // Create the new user
            var user = await _userService.CreateUserAsync(model);

            // create the session for the user			
            var sessionInfo = await _sessionService.CreateSession(user.Id);

            return Created("/api/signup", sessionInfo);
        }
    }
}
