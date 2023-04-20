using System.ComponentModel.DataAnnotations;
using DeliveryBackend.Data.Models.Enums;

namespace DeliveryBackend.DTO;

public class OrderDto
{
    public Guid Id { get; set; }
    [Required]
    public DateTime DeliveryTime { get; set; }
    [Required]
    public DateTime OrderTime { get; set; }
    [Required]
    public string Status { get; set; }
    [Required]
    public double Price { get; set; }
    [Required]
    public List<DishBasketDto?> Dishes { get; set; }
    [Required]
    [MinLength(1)]
    public string Address { get; set; }
}