namespace Backend.DAL.Entities;

public class Cook
{
    public Guid Id { get; set; }

    public List<Order> Orders { get; set; } = new();

    public Restaurant? Restaurant { get; set; }
}