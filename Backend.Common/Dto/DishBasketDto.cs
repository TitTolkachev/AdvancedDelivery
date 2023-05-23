using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class DishBasketDto
{
    public Guid Id { get; set; }
    [Required] [MinLength(1)] public string Name { get; set; } = null!;

    [Required] public decimal Price { get; set; }
    [Required] public decimal TotalPrice { get; set; }
    [Required] public int Amount { get; set; }
    public string? Image { get; set; }
}