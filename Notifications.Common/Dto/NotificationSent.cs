using System.ComponentModel.DataAnnotations;

namespace Notifications.Common.Dto;

public class NotificationSent
{
    [Required] public Guid Id { get; set; }
    [Required] public Guid UserId { get; set; }
    [Required] public Guid OrderId { get; set; }
    [Required] public string Text { get; set; } = string.Empty;
    [Required] public Status Status { get; set; }
}