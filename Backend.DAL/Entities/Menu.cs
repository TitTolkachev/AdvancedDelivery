using System.ComponentModel.DataAnnotations;

namespace Backend.DAL.Entities;

public class Menu
{
    public Guid Id { get; set; }

    [Required] public string Name { get; set; } = null!;

    [Required] public Restaurant Restaurant { get; set; } = null!;
    
    public List<Dish> Dishes { get; set; } = new();
}