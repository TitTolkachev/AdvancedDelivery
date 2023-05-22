using System.Text;
using System.Text.Json;
using Backend.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Backend.BL.Services;

public class ProducerService : IProducerService
{
    private readonly IRabbitMqService _connection;
    private readonly IConfiguration _configuration;

    public ProducerService(IRabbitMqService connection, IConfiguration configuration)
    {
        _connection = connection;
        _configuration = configuration;
    }

    public void SendMessage<T>(T message)
    {
        using var channel = _connection.CreateChannel().CreateModel();

        var queue = _configuration.GetSection("MqConfiguration:QueueName").Get<string>();
        var exchange = _configuration.GetSection("MqConfiguration:ExchangeName").Get<string>();

        channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);

        var jsonStr = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonStr);

        channel.BasicPublish(exchange, queue, body: body);
    }
}