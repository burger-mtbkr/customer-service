using Customer.Service.Models;
using Customer.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Xml.XPath;

namespace Customer.Service.Controllers
{
    [DisableCors]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController: ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Returns all customers
        /// </summary>
        /// <returns></returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<CustomerModel>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<CustomerModel>> Search([FromQuery] GetCustomerRequest request)
        {
            var result = _customerService.GetAllCustomers(request);
            return Ok(result);
        }

        /// <summary>
        /// Gets a single customer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerModel?> Get(string id)
        {
            var customer = _customerService.GetCustomerByID(id);
            return Ok(customer);
        }

        /// <summary>
        /// Create a customer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerModel>> Post([FromBody] CustomerModel model)
        {
            var newCustomer = await _customerService.CreateCustomerAsync(model);
            return Created("/api/customer", newCustomer);
        }

        /// <summary>
        /// Edit a customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(string id, [FromBody] CustomerModel model)
        {
            await _customerService.UpdateCustomerAsync(id, model);
            return NoContent();
        }

        /// <summary>
        /// Edit a customer's status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Patch(string id, [FromBody] CustomerStatusUpdateRequest model)
        {
            await _customerService.UpdateCustomerStatusAsync(id, model);
            return Accepted();
        }

        /// <summary>
        /// Delete a customer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            await _customerService.DeleteCustomerAsync(id);
            return NoContent();
        }
    }
}
