using System.ComponentModel.DataAnnotations;

namespace Backend.DAL.Entities;

public class Dish
{
    public Guid Id { get; set; }
    [Required] [MinLength(1)] public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Required] public decimal Price { get; set; }
    public double? Rating { get; set; }
    public string Image { get; set; } = null!;

    [Required] public bool Vegetarian { get; set; }
    [Required] public string Category { get; set; } = null!;

    public List<Cart> Carts { get; set; } = null!;
    public List<Rating> Ratings { get; set; } = null!;
}