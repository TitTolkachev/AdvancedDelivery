namespace Backend.DAL.Entities;

public class User
{
    public Guid Id { get; set; }    
    public string? Address { get; set; }

    public List<Rating> Ratings { get; set; } = null!;
    public List<Order> Orders { get; set; } = null!;
    public List<Cart> Carts { get; set; } = null!;
}