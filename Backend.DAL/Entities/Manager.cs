using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DAL.Entities;

public class Manager
{
    public Guid Id { get; set; }

    public Guid RestaurantId { get; set; }

    [ForeignKey("RestaurantId")] public Restaurant? Restaurant { get; set; }
}