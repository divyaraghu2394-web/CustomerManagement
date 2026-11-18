
using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Models;

namespace OrderManagement.API.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<KafkaCustomer> KafkaCustomers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .Property(o => o.Amount)
            .HasPrecision(18, 2);

        // Existing Customer table remains unchanged
        modelBuilder.Entity<Customer>()
            .ToTable("Customers");

        // Separate table for Kafka customer events
        modelBuilder.Entity<KafkaCustomer>()
            .ToTable("KafkaCustomers");

        modelBuilder.Entity<KafkaCustomer>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<KafkaCustomer>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<KafkaCustomer>()
            .HasIndex(c => c.CustomerId)
            .IsUnique();
    }
}
