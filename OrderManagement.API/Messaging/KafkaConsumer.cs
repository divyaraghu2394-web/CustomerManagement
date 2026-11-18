
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Messaging.Contracts;
using OrderManagement.API.Data;
using OrderManagement.API.Models;

namespace OrderManagement.API.Messaging;

public class KafkaConsumer : BackgroundService
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public KafkaConsumer(
        ILogger<KafkaConsumer> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "order-management-group-2",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        using var consumer =
            new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe("customer-events");

        _logger.LogInformation(
            "Kafka Consumer listening to customer-events");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);

                var customerEvent =
                    JsonSerializer.Deserialize<CustomerEvent>(
                        result.Message.Value);

                if (customerEvent == null)
                    continue;

                await using var scope =
                    _scopeFactory.CreateAsyncScope();

                var dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<OrderDbContext>();

                // Check existing customer in KafkaCustomers
                var customer = await dbContext.KafkaCustomers
                    .FirstOrDefaultAsync(
                        c => c.CustomerId == customerEvent.CustomerId,
                        stoppingToken);

                if (customer == null)
                {
                    // Insert new Kafka customer
                    customer = new KafkaCustomer
                    {
                        CustomerId = customerEvent.CustomerId,
                        CustomerName = customerEvent.Name,
                        Email = customerEvent.Email,
                        Mobile = string.Empty,
                        City = string.Empty,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true
                    };

                    dbContext.KafkaCustomers.Add(customer);

                    _logger.LogInformation(
                        "New customer received. Customer ID: {CustomerId}",
                        customerEvent.CustomerId);
                }
                else
                {
                    // Update existing Kafka customer
                    customer.CustomerName = customerEvent.Name;
                    customer.Email = customerEvent.Email;

                    _logger.LogInformation(
                        "Existing customer updated. Customer ID: {CustomerId}",
                        customerEvent.CustomerId);
                }

                await dbContext.SaveChangesAsync(stoppingToken);

                _logger.LogInformation(
                    "Customer saved successfully. Customer ID: {CustomerId}",
                    customer.CustomerId);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka Consumer stopped.");
        }
        finally
        {
            consumer.Close();
        }
    }
}
