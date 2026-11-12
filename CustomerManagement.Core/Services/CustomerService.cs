
using CustomerManagement.Core.DTOs;
using CustomerManagement.Core.Interfaces;
using CustomerManagement.Core.Models;

namespace CustomerManagement.Core.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            return await _repository.AddAsync(customer);
        }

        public async Task<Customer?> UpdateAsync(int id, Customer customer)
        {
            return await _repository.UpdateAsync(id, customer);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<PagedResult<Customer>> GetPagedAsync(
      int pageNumber,
      int pageSize,
      string? search,
      string? city)
        {
            return await _repository.GetPagedAsync(
                pageNumber,
                pageSize,
                search,
                city);
        }
    }
}
