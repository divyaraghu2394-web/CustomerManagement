
using Asp.Versioning;
using CustomerManagement.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.API.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/Customer")]
    public class CustomerV2Controller : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerV2Controller(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/v2/Customer
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllAsync();

            var response = customers.Select(c => new
            {
                id = c.CustomerId,
                name = c.CustomerName,
                emailAddress = c.Email,
                city = c.City,
                status = c.IsActive ? "Active" : "Inactive"
            });

            return Ok(response);
        }
    }
}
