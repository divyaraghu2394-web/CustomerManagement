
using CustomerManagement.Core.Interfaces;
using CustomerManagement.Core.Models;
using CustomerManagement.Core.Services;
using Moq;

namespace CustomerManagement.Tests.Services
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _mockRepository;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _mockRepository = new Mock<ICustomerRepository>();
            _service = new CustomerService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer
                {
                    CustomerId = 1,
                    CustomerName = "Priya",
                    Email = "priya@gmail.com",
                    City = "Chennai"
                }
            };

            _mockRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(customers);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Priya", result.First().CustomerName);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCustomer()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerId = 1,
                CustomerName = "Priya"
            };

            _mockRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(customer);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Priya", result.CustomerName);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Customer?)null);

            // Act
            var result = await _service.GetByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenCustomerExists()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.DeleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
        }
    }
}
