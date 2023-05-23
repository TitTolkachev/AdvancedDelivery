namespace Backend.DAL.Entities;

public class Courier
{
    public Guid Id { get; set; }

    public List<Order> Orders { get; set; } = new();
}