
using CustomerManagement.Core.DTOs;
using CustomerManagement.Core.Models;


namespace CustomerManagement.Core.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(int id);

        Task<Customer> AddAsync(Customer customer);

        Task<Customer?> UpdateAsync(int id, Customer customer);

        Task<bool> DeleteAsync(int id);

        Task<PagedResult<Customer>> GetPagedAsync(
    int pageNumber,
    int pageSize,
    string? search,
    string? city);
    }
}
