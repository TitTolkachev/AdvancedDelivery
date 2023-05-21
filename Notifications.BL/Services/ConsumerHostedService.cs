using Microsoft.Extensions.Hosting;
using Notifications.Common.Interfaces;

namespace Notifications.BL.Services;

public class ConsumerHostedService : BackgroundService
{
    private readonly IConsumerService _consumerService;

    public ConsumerHostedService(IConsumerService consumerService)
    {
        _consumerService = consumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumerService.ReadMessages();
    }
}