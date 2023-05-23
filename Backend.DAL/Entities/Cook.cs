using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DAL.Entities;

public class Cook
{
    public Guid Id { get; set; }

    public List<Order> Orders { get; set; } = new();

    public Guid RestaurantId { get; set; }

    [ForeignKey("RestaurantId")] public Restaurant? Restaurant { get; set; }
}