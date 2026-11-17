
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Data;
using OrderManagement.API.Models;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _context;

    public OrdersController(OrderDbContext context)
    {
        _context = context;
    }

    // GET: api/orders
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.Orders.ToListAsync();
        return Ok(orders);
    }

    // GET: api/orders/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            return NotFound("Order not found.");

        return Ok(order);
    }

    // POST: api/orders
    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        order.OrderDate = DateTime.UtcNow;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.OrderId },
            order);
    }

    // PUT: api/orders/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Order updatedOrder)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            return NotFound("Order not found.");

        order.CustomerId = updatedOrder.CustomerId;
        order.ProductName = updatedOrder.ProductName;
        order.Quantity = updatedOrder.Quantity;
        order.Amount = updatedOrder.Amount;
        order.Status = updatedOrder.Status;

        await _context.SaveChangesAsync();

        return Ok(order);
    }

    // DELETE: api/orders/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            return NotFound("Order not found.");

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return Ok("Order deleted successfully.");
    }
}
