using Notifications.Common.Dto;

namespace Notifications.Common.Interfaces;

public interface INotificationsService
{
    public Task AcceptAndSend(NotificationReceived notification);
}