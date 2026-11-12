
using AutoMapper;
using CustomerManagement.Core.DTOs;
using CustomerManagement.Core.Models;
using CustomerManagement.Core.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace CustomerManagement.API.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly IValidator<CustomerCreateDto> _createValidator;
        private readonly IValidator<CustomerUpdateDto> _updateValidator;

        private readonly ILogger<CustomerController> _logger;

        public CustomerController(
      ICustomerService customerService,
      IMapper mapper,
      IValidator<CustomerCreateDto> createValidator,
      IValidator<CustomerUpdateDto> updateValidator,
      ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GetAll customer API started");

            var customers = await _customerService.GetAllAsync();

            var response = _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);

            _logger.LogInformation(
                "GetAll customer API completed. Total customers: {Count}",
                response.Count()
            );

            return Ok(response);
        }

        // GET: api/Customer/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);

            if (customer == null)
                return NotFound("Customer not found");

            var response = _mapper.Map<CustomerResponseDto>(customer);
            return Ok(response);
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<IActionResult> Create(CustomerCreateDto customerDto)
        {
            var validationResult = await _createValidator.ValidateAsync(customerDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var customer = _mapper.Map<Customer>(customerDto);
            var result = await _customerService.AddAsync(customer);
            var response = _mapper.Map<CustomerResponseDto>(result);

            return CreatedAtAction(
                nameof(GetById),
                new { id = response.CustomerId },
                response
            );
        }

        // PUT: api/Customer/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            CustomerUpdateDto customerDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(customerDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var customer = _mapper.Map<Customer>(customerDto);
            var result = await _customerService.UpdateAsync(id, customer);

            if (result == null)
                return NotFound("Customer not found");

            var response = _mapper.Map<CustomerResponseDto>(result);
            return Ok(response);
        }

        // DELETE: api/Customer/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteAsync(id);

            if (!result)
                return NotFound("Customer not found");

            return Ok("Customer deleted successfully");
        }

        // GET: api/v1/Customer/paged
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
       int pageNumber = 1,
       int pageSize = 5,
       string? search = null,
       string? city = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(
                    "Page number must be at least 1 and page size must be between 1 and 100.");
            }

            var result = await _customerService.GetPagedAsync(
                pageNumber,
                pageSize,
                search,
                city);

            var response = new PagedResult<CustomerResponseDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalRecords = result.TotalRecords,
                TotalPages = result.TotalPages,
                Data = _mapper.Map<IEnumerable<CustomerResponseDto>>(result.Data)
            };

            return Ok(response);
        }
    }
}
