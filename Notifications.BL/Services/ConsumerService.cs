using Microsoft.Extensions.Configuration;
using Notifications.Common.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Notifications.BL.Services;

public class ConsumerService : IConsumerService, IDisposable
{
    private readonly IModel _model;
    private readonly IConnection _connection;

    private static IConfiguration _configuration = null!;

    public ConsumerService(IRabbitMqService rabbitMqService, IConfiguration configuration)
    {
        _configuration = configuration;
        _connection = rabbitMqService.CreateChannel();
        _model = _connection.CreateModel();
        _model.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
        _model.ExchangeDeclare(_configuration.GetSection("MqConfiguration:my_exchange_name").Get<string>(),
            ExchangeType.Fanout, durable: true, autoDelete: false);
        _model.QueueBind(QueueName, _configuration.GetSection("MqConfiguration:my_exchange_name").Get<string>(),
            string.Empty);
    }

    private static readonly string QueueName = _configuration.GetSection("MqConfiguration:my_queue_name").Get<string>();

    public async Task ReadMessages()
    {
        var consumer = new AsyncEventingBasicConsumer(_model);
        consumer.Received += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var text = System.Text.Encoding.UTF8.GetString(body);
            Console.WriteLine(text);
            
            // TODO(Сделать обращение к сервису с веб-сокетами)
            // 
            
            await Task.CompletedTask;
            _model.BasicAck(ea.DeliveryTag, false);
        };
        _model.BasicConsume(QueueName, false, consumer);
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_model.IsOpen)
            _model.Close();
        if (_connection.IsOpen)
            _connection.Close();
    }
}