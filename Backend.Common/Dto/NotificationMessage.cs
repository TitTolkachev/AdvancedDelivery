using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class NotificationMessage
{
    [Required] public Guid UserId { get; set; }
    [Required] public Guid OrderId { get; set; }
    [Required] public string Text { get; set; } = string.Empty;
    [Required] public NotificationStatus Status { get; set; }
}