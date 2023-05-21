using RabbitMQ.Client;

namespace Notifications.Common.Interfaces;

public interface IRabbitMqService
{
    IConnection CreateChannel();
}