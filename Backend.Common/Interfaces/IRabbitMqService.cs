using RabbitMQ.Client;

namespace Backend.Common.Interfaces;

public interface IRabbitMqService
{
    IConnection CreateChannel();
}