using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyMediator.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Messaging.Contracts;
using DocumentsBusinessLogic.UseCases.DocumentsUseCases;

namespace BackgroundJobs.Services;

public class DocumentCreatedConsumer : BackgroundService
{
    private readonly IConnection _rabbitConnection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _exchangeName;
    private readonly string _queueName;
    private readonly string _routingKey;
    private IModel? _channel;

    public DocumentCreatedConsumer( IConnection rabbitConnection,IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        var connectionArg = rabbitConnection;
        var scopeFactoryArg = scopeFactory;

        var exchangeFromConfig = configuration["RabbitMq:Exchange"];
        var queueNameFromConfig = configuration["RabbitMq:QueueName"];
        var routingKeyFromConfig = configuration["RabbitMq:RoutingKey"];

        if (string.IsNullOrWhiteSpace(exchangeFromConfig))
        {
            throw new InvalidOperationException("RabbitMq:Exchange not configured");
        }

        if (string.IsNullOrWhiteSpace(queueNameFromConfig))
        {
            queueNameFromConfig = "documents.create.queue";
        }

        if (string.IsNullOrWhiteSpace(routingKeyFromConfig))
        {
            routingKeyFromConfig = "document.create";
        }

        _rabbitConnection = connectionArg;
        _scopeFactory = scopeFactoryArg;
        _exchangeName = exchangeFromConfig;
        _queueName = queueNameFromConfig;
        _routingKey = routingKeyFromConfig;
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        InitializeChannel();
        RegisterConsumer();

        return Task.CompletedTask;
    }

    private void InitializeChannel()
    {
        _channel = _rabbitConnection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: _exchangeName,
            type: ExchangeType.Direct,
            durable: true);

        _channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        _channel.QueueBind(
            queue: _queueName,
            exchange: _exchangeName,
            routingKey: _routingKey);
    }

    private void RegisterConsumer()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel!);
        consumer.Received += OnMessageAsync;

        _channel!.BasicConsume(
            queue: _queueName,
            autoAck: true,
            consumer: consumer);
    }

    private async Task OnMessageAsync(object sender, BasicDeliverEventArgs ea)
    {
        var rawBody = ea.Body.ToArray();
        var messageJson = Encoding.UTF8.GetString(rawBody);
        var createMsg = JsonSerializer.Deserialize<CreateDocumentMessage>(messageJson);

        if (createMsg is null)
            return;

        var fileBytes = Convert.FromBase64String(createMsg.ContentBase64);

        using var memoryStream = new MemoryStream(fileBytes);
        IFormFile uploadedFile = new FormFile(
            baseStream: memoryStream,
            baseStreamOffset: 0,
            length: memoryStream.Length,
            name: "file",
            fileName: createMsg.FileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = createMsg.ContentType
        };

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var request = new CreateDocumentRequest(uploadedFile);

        await mediator.Send(request);
    }

    public override void Dispose()
    {
        if (_channel is not null)
        {
            _channel.Close();
            _channel.Dispose();
        }

        base.Dispose();
    }
}
