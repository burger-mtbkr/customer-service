using Customer.Service.Models;
using Customer.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Service.Controllers
{
    [DisableCors]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsController: ControllerBase
    {
        private readonly ILeadsService _leadsService;

        public LeadsController(ILeadsService customerService)
        {
            _leadsService = customerService;
        }

        /// <summary>
        /// Get all leads for a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(IEnumerable<LeadModel>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<LeadModel>> GetLeads(string customerId)
        {
            var result = _leadsService.GetLeads(customerId);
            return Ok(result);
        }

        /// <summary>
        /// Get a lead by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LeadModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<LeadModel?> GetLeadById(string id)
        {
            var customer = _leadsService.GetLeadById(id);
            return Ok(customer);
        }

        /// <summary>
        /// Create a new lead for a customer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LeadModel>> Post([FromBody] LeadModel model)
        {
            var newCustomer = await _leadsService.CreateLeadAsync(model);
            return Created("/api/leads", newCustomer);
        }

        /// <summary>
        /// Update a lead
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(string id, [FromBody] LeadModel model)
        {
            await _leadsService.UpdateLeadAsync(id, model);
            return NoContent();
        }
    }
}
