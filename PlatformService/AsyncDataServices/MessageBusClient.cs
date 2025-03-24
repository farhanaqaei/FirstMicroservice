using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient, IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly Task _initializationTask;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;
        _factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"])
        };

        _initializationTask = Task.Run(() => InitializeAsync())
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                        Console.WriteLine($"--> Initialization error: {t.Exception.Flatten().Message}");
                });
    }

    private async Task InitializeAsync()
    {
        try
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);

            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;

            Console.WriteLine("--> Connected to Message Bus");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
        }
    }

    public async ValueTask PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        await _initializationTask;

        var message = JsonSerializer.Serialize(platformPublishedDto);

        if (_connection != null && _connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
            await SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ connection is closed, not sending");
        }
    }

    private async Task SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        if (_channel == null || !_channel.IsOpen)
        {
            Console.WriteLine("--> Cannot send message, channel is null");
            return;
        }

        await _channel.BasicPublishAsync(
            exchange: "trigger",
            routingKey: "",
            // basicProperties: null,
            body: body);

        Console.WriteLine($"--> We have sent {message}");
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("--> MessageBus Disposed");

        if (_channel != null && _channel.IsOpen)
            await _channel.CloseAsync();

        if (_connection != null && _connection.IsOpen)
            await _connection.CloseAsync();
    }

    private Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ Connection Shutdown");
        return Task.CompletedTask;
    }

}
