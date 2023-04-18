using Customer.Service.Models;
using Customer.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController: ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        /// <summary>
		/// Login returns a new session token
		/// </summary>
		/// <param name="request"></param>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> Post([FromBody] LoginRequest request)
        {
            if(request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email or password has not been provoded");
            }

            var token = await _loginService.Login(request);          
            return Created("/api/login", token);
        }

        /// <summary>
        /// Logout - deletes the session
        /// </summary>
        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete()
        {
            await _loginService.Logout();
            return NoContent();
        }
    }
}
