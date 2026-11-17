
using System.Text;
using System.Text.Json;
using Messaging.Contracts;
using RabbitMQ.Client;

namespace CustomerManagement.API.Messaging;

public class RabbitMqPublisher : IDisposable
{
    private readonly IConnection _connection;

    public RabbitMqPublisher()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
    }

    public void PublishCustomerCreated(CustomerCreatedEvent customerEvent)
    {
        using var channel = _connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: "customer-events",
            type: ExchangeType.Direct,
            durable: true
        );

        var message = JsonSerializer.Serialize(customerEvent);
        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        channel.BasicPublish(
            exchange: "customer-events",
            routingKey: "customer.created",
            basicProperties: properties,
            body: body
        );

        Console.WriteLine($"Published customer event: {customerEvent.CustomerId}");
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
