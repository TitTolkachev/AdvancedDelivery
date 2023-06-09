﻿using Microsoft.Extensions.Configuration;
using Notifications.BL.Configuration;
using Notifications.Common.Interfaces;
using RabbitMQ.Client;

namespace Notifications.BL.Services;

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
            DispatchConsumersAsync = true
        };
        var channel = connection.CreateConnection();
        return channel;
    }
}