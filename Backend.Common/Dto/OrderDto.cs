using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class OrderDto
{
    public Guid Id { get; set; }
    [Required] public DateTime DeliveryTime { get; set; }
    [Required] public DateTime OrderTime { get; set; }
    [Required] public string Status { get; set; } = null!;
    [Required] public decimal Price { get; set; }
    [Required] public List<DishBasketDto?> Dishes { get; set; } = null!;
    [Required] [MinLength(1)] public string Address { get; set; } = null!;
}