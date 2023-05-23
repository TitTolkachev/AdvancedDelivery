using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class OrderRepeatDto
{
    [Required] public Guid OrderId { get; set; }
    [Required] public Guid RestaurantId { get; set; }
    [Required] public DateTime DeliveryTime { get; set; }
    [Required] [MinLength(1)] public string Address { get; set; } = null!;
}