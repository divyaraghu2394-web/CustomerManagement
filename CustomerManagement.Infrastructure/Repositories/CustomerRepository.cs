
using CustomerManagement.Core.DTOs;
using CustomerManagement.Core.Interfaces;
using CustomerManagement.Core.Models;
using CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all customers
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        // Get customer by ID
        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        // Add customer
        public async Task<Customer> AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        // Update customer
        public async Task<Customer?> UpdateAsync(int id, Customer customer)
        {
            var existingCustomer = await GetByIdAsync(id);

            if (existingCustomer == null)
                return null;

            existingCustomer.CustomerName = customer.CustomerName;
            existingCustomer.Email = customer.Email;
            existingCustomer.Mobile = customer.Mobile;
            existingCustomer.City = customer.City;
            existingCustomer.IsActive = customer.IsActive;

            await _context.SaveChangesAsync();

            return existingCustomer;
        }

        // Delete customer
        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await GetByIdAsync(id);

            if (customer == null)
                return false;

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<PagedResult<Customer>> GetPagedAsync(
     int pageNumber,
     int pageSize,
     string? search,
     string? city)
        {
            var query = _context.Customers
                .AsNoTracking()
                .AsQueryable();

            // Search by customer name
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.CustomerName.Contains(search));
            }

            // Filter by city
            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(c =>
                    c.City.Contains(city));
            }

            var totalRecords = await query.CountAsync();

            var customers = await query
                .OrderBy(c => c.CustomerId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Customer>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(
                    (double)totalRecords / pageSize),
                Data = customers
            };
        }
    }
}
