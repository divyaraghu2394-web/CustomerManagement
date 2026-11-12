
using AutoMapper;
using CustomerManagement.API.Controllers;
using CustomerManagement.Core.DTOs;
using CustomerManagement.Core.Models;
using CustomerManagement.Core.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerManagement.Tests.Controllers
{
    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerService> _mockService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IValidator<CustomerCreateDto>> _mockCreateValidator;
        private readonly Mock<IValidator<CustomerUpdateDto>> _mockUpdateValidator;

        private readonly CustomerController _controller;

        private readonly Mock<ILogger<CustomerController>> _mockLogger;

        public CustomerControllerTests()
        {
            _mockService = new Mock<ICustomerService>();
            _mockMapper = new Mock<IMapper>();
            _mockCreateValidator = new Mock<IValidator<CustomerCreateDto>>();
            _mockUpdateValidator = new Mock<IValidator<CustomerUpdateDto>>();
            _mockLogger = new Mock<ILogger<CustomerController>>();

            _controller = new CustomerController(
      _mockService.Object,
      _mockMapper.Object,
      _mockCreateValidator.Object,
      _mockUpdateValidator.Object,
      _mockLogger.Object
  );
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var customers = new List<Customer>
            {
                new Customer
                {
                    CustomerId = 1,
                    CustomerName = "Priya"
                }
            };

            var response = new List<CustomerResponseDto>
            {
                new CustomerResponseDto
                {
                    CustomerId = 1,
                    CustomerName = "Priya"
                }
            };

            _mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(customers);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<CustomerResponseDto>>(customers))
                .Returns(response);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(99))
                .ReturnsAsync((Customer?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCustomerExists()
        {
            var customer = new Customer
            {
                CustomerId = 1,
                CustomerName = "Priya"
            };

            var response = new CustomerResponseDto
            {
                CustomerId = 1,
                CustomerName = "Priya"
            };

            _mockService.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(customer);

            _mockMapper
                .Setup(m => m.Map<CustomerResponseDto>(customer))
                .Returns(response);

            var result = await _controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction()
        {
            var dto = new CustomerCreateDto
            {
                CustomerName = "Arun",
                Email = "arun@gmail.com",
                Mobile = "9876543210",
                City = "Chennai"
            };

            var customer = new Customer
            {
                CustomerId = 2,
                CustomerName = "Arun"
            };

            var response = new CustomerResponseDto
            {
                CustomerId = 2,
                CustomerName = "Arun"
            };

            _mockCreateValidator
                .Setup(v => v.ValidateAsync(
                    dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockMapper.Setup(m => m.Map<Customer>(dto))
                .Returns(customer);

            _mockService.Setup(s => s.AddAsync(customer))
                .ReturnsAsync(customer);

            _mockMapper
                .Setup(m => m.Map<CustomerResponseDto>(customer))
                .Returns(response);

            var result = await _controller.Create(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdResult.Value);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenValidationFails()
        {
            var dto = new CustomerCreateDto
            {
                CustomerName = ""
            };

            var validationResult = new ValidationResult
            {
                Errors =
                {
                    new ValidationFailure(
                        "CustomerName",
                        "Customer name is required.")
                }
            };

            _mockCreateValidator
                .Setup(v => v.ValidateAsync(
                    dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var result = await _controller.Create(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnOk()
        {
            var dto = new CustomerUpdateDto
            {
                CustomerName = "Priya Updated",
                Email = "priya@gmail.com",
                Mobile = "9876543210",
                City = "Chennai",
                IsActive = true
            };

            var customer = new Customer
            {
                CustomerId = 1,
                CustomerName = "Priya Updated"
            };

            _mockUpdateValidator
                .Setup(v => v.ValidateAsync(
                    dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockMapper.Setup(m => m.Map<Customer>(dto))
                .Returns(customer);

            _mockService.Setup(s => s.UpdateAsync(1, customer))
                .ReturnsAsync(customer);

            _mockMapper
                .Setup(m => m.Map<CustomerResponseDto>(customer))
                .Returns(new CustomerResponseDto
                {
                    CustomerId = 1,
                    CustomerName = "Priya Updated"
                });

            var result = await _controller.Update(1, dto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound()
        {
            var dto = new CustomerUpdateDto
            {
                CustomerName = "Unknown",
                Email = "unknown@gmail.com",
                Mobile = "9876543210",
                City = "Chennai",
                IsActive = true
            };

            var customer = new Customer
            {
                CustomerName = "Unknown"
            };

            _mockUpdateValidator
                .Setup(v => v.ValidateAsync(
                    dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockMapper.Setup(m => m.Map<Customer>(dto))
                .Returns(customer);

            _mockService.Setup(s => s.UpdateAsync(99, customer))
                .ReturnsAsync((Customer?)null);

            var result = await _controller.Update(99, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnOk()
        {
            _mockService.Setup(s => s.DeleteAsync(1))
                .ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound()
        {
            _mockService.Setup(s => s.DeleteAsync(99))
                .ReturnsAsync(false);

            var result = await _controller.Delete(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
