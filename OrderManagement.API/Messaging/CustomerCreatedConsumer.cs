
using System.Text;
using System.Text.Json;
using Messaging.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderManagement.API.Messaging;

public class CustomerCreatedConsumer : BackgroundService
{
    private readonly ILogger<CustomerCreatedConsumer> _logger;

    private IConnection? _connection;
    private IModel? _channel;

    public CustomerCreatedConsumer(
        ILogger<CustomerCreatedConsumer> logger)
    {
        _logger = logger;
    }

    protected override Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: "customer-events",
            type: ExchangeType.Direct,
            durable: true
        );

        _channel.QueueDeclare(
            queue: "order-customer-created",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueBind(
            queue: "order-customer-created",
            exchange: "customer-events",
            routingKey: "customer.created"
        );

        _channel.BasicQos(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false
        );

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (sender, eventArgs) =>
        {
            try
            {
                // Read JSON from RabbitMQ
                var json = Encoding.UTF8.GetString(
                    eventArgs.Body.ToArray()
                );

                // Display original JSON
                _logger.LogInformation(
                    "Received JSON Message: {Json}",
                    json
                );

                // Convert JSON into C# object
                var customerEvent =
                    JsonSerializer.Deserialize<CustomerCreatedEvent>(json);

                if (customerEvent == null)
                {
                    throw new InvalidOperationException(
                        "Customer event deserialization returned null."
                    );
                }

                // Display individual properties
                _logger.LogInformation(
                    "Customer ID: {CustomerId}",
                    customerEvent.CustomerId
                );

                _logger.LogInformation(
                    "Customer Name: {CustomerName}",
                    customerEvent.CustomerName
                );

                _logger.LogInformation(
                    "Email: {Email}",
                    customerEvent.Email
                );

                _logger.LogInformation(
                    "Created At: {CreatedAt}",
                    customerEvent.CreatedAt
                );

                // Acknowledge successful processing
                _channel.BasicAck(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing customer event"
                );

                _channel.BasicNack(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false
                );
            }
        };

        _channel.BasicConsume(
            queue: "order-customer-created",
            autoAck: false,
            consumer: consumer
        );

        _logger.LogInformation(
            "Customer Created Consumer Started..."
        );

        return Task.Delay(
            Timeout.Infinite,
            stoppingToken
        );
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();

        _channel?.Dispose();
        _connection?.Dispose();

        base.Dispose();
    }
}
