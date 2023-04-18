using Customer.Service.Exceptions;
using Customer.Service.Models;
using Customer.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Service.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Returns all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<UserResponse>> Get()
        {
            var result = _userService.GetAllUsers();
            return Ok(result);
        }

        /// <summary>
        /// Gets a single user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserNotFoundException), StatusCodes.Status404NotFound)]
        public ActionResult<UserResponse> Get(string id)
        {
            var user = _userService.GetUser(id);
            return Ok(user);
        }

        /// <summary>
        /// Edit a user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(UserNotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ArgumentNullException), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put(string id, [FromBody] UserModel model)
        {
            await _userService.EditUserAsync(id, model);
            return NoContent();
        }

        /// <summary>
        /// Delete a user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ArgumentNullException), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(string id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Change Password for a user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(UserNotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ArgumentNullException), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Patch(string id, [FromBody] PasswordChangeRequest request)
        {
            await _userService.ChangePasswordAsync(id, request);
            return NoContent();
        }
    }
}
