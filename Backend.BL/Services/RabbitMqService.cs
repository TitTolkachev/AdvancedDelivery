using Backend.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Notifications.BL.Configuration;
using RabbitMQ.Client;

namespace Backend.BL.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly RabbitMqConfiguration _configuration;

    public RabbitMqService(IConfiguration configuration)
    {
        _configuration = new RabbitMqConfiguration(
            configuration.GetSection("RabbitMqConfiguration:HostName").Get<string>(),
            configuration.GetSection("RabbitMqConfiguration:Username").Get<string>(),
            configuration.GetSection("RabbitMqConfiguration:Password").Get<string>()
        );
    }

    public IConnection CreateChannel()
    {
        var connection = new ConnectionFactory
        {
            UserName = _configuration.Username,
            Password = _configuration.Password,
            HostName = _configuration.HostName,
            VirtualHost = "/"
        };
        var channel = connection.CreateConnection();
        return channel;
    }
}