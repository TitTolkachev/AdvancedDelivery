using Microsoft.AspNetCore.SignalR;
using Notifications.BL.Hubs;
using Notifications.Common.Dto;
using Notifications.Common.Interfaces;
using Notifications.DAL;
using Notifications.DAL.Entities;
using Status = Notifications.Common.Dto.Status;

namespace Notifications.BL.Services;

public class NotificationsService : INotificationsService
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationsHub> _hubContext;

    public NotificationsService(AppDbContext context, IHubContext<NotificationsHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task AcceptAndSend(NotificationReceived notification)
    {
        await _context.Notifications.AddAsync(new Notification
        {
            Id = Guid.NewGuid(),
            OrderId = notification.OrderId,
            Status = notification.Status == Status.New ? DAL.Entities.Status.New : DAL.Entities.Status.Sent,
            Text = notification.Text,
            UserId = notification.UserId
        });
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("Receive Notification", notification);
    }
}