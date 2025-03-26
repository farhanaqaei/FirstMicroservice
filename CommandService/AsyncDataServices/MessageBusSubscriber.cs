
using System.Text;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService, IAsyncDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IChannel _channel;
    private string _queueName;
    private readonly Task _initializationTask;

    public MessageBusSubscriber(
        IConfiguration configuration, 
        IEventProcessor eventProcessor
        )
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;

        _initializationTask = Task.Run(() => InitializeRabbitMQ())
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                        Console.WriteLine($"--> Initialization error: {t.Exception.Flatten().Message}");
                });
    }

    private async Task InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"])
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);
        var queueDeclare = await _channel.QueueDeclareAsync();
        _queueName = queueDeclare.QueueName;
        await _channel.QueueBindAsync(queue: _queueName, exchange: "trigger", routingKey: "");

        Console.WriteLine("--> Listening on the Message Bus...");

        _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _initializationTask;

        stoppingToken.ThrowIfCancellationRequested();
        
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (ModuleHandle, ea) =>
        {
            Console.WriteLine("--> Event Received!");

            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            _eventProcessor.ProcessEvent(notificationMessage);
        };

        await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);
    }

    private Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> Connection Shutdown");
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("--> MessageBus Disposed");

        if (_channel != null && _channel.IsOpen)
            await _channel.CloseAsync();

        if (_connection != null && _connection.IsOpen)
            await _connection.CloseAsync();
    }
}
