using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DAL.Entities;

public class Order
{
    public Guid Id { get; set; }
    [Required] public int Number { get; set; }
    [Required] public DateTime DeliveryTime { get; set; }
    [Required] public DateTime OrderTime { get; set; }
    [Required] public string Status { get; set; } = null!;
    [Required] public decimal Price { get; set; }
    [Required] [MinLength(1)] public string Address { get; set; } = null!;

    [Required] public Guid UserId { get; set; }
    [Required] [ForeignKey("UserId")] public User User { get; set; } = null!;
    [Required] public Guid RestaurantId { get; set; }

    [Required]
    [ForeignKey("RestaurantId")]
    public Restaurant Restaurant { get; set; } = null!;

    public List<Cart> Carts { get; set; } = null!;
}