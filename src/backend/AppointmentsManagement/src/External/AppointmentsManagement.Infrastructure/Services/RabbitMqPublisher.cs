using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using AppointmentsManagement.Application.Common.Interfaces.IServices;

namespace AppointmentsManagement.Infrastructure.Messaging;

public class RabbitMqPublisher : IMessagingPublisher, IDisposable
{
    private readonly IModel _channel;
    private readonly string _exchangeName;

    private const string ExchangeTypeName = ExchangeType.Direct;
    private const bool ExchangeIsDurable = true;
    private const byte PersistentDeliveryMode = 2;

    public RabbitMqPublisher(IConnection rabbitConnection, IConfiguration configuration)
    {
        var exchangeFromConfig = configuration["RabbitMq:Exchange"];

        if (string.IsNullOrWhiteSpace(exchangeFromConfig))
        {
            throw new InvalidOperationException("RabbitMq:Exchange not configured");
        }

        _exchangeName = exchangeFromConfig;
        _channel = rabbitConnection.CreateModel();

        DeclareExchange();
    }

    public Task PublishAsync<T>(string routingKey, T message)
    {
        var jsonPayload = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonPayload);

        var basicProperties = _channel.CreateBasicProperties();
        basicProperties.ContentType = "application/json";
        basicProperties.DeliveryMode = PersistentDeliveryMode;

        _channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: routingKey,
            basicProperties: basicProperties,
            body: body
        );

        return Task.CompletedTask;
    }

    private void DeclareExchange()
    {
        _channel.ExchangeDeclare(
            exchange: _exchangeName,
            type: ExchangeTypeName,
            durable: ExchangeIsDurable
        );
    }

    public void Dispose()
    {
        _channel.Close();
        _channel.Dispose();
    }
}
