using System.ComponentModel.DataAnnotations;

namespace Backend.DAL.Entities;

public class Restaurant
{
    public Guid Id { get; set; }

    [Required] public string Name { get; set; } = null!;

    public List<Order> Orders { get; set; } = new();

    public List<Menu> Menus { get; set; } = new();

    public List<Cook> Cooks { get; set; } = new();

    public List<Manager> Managers { get; set; } = new();
}