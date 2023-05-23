namespace Backend.DAL.Entities;

public class Manager
{
    public Guid Id { get; set; }

    public Restaurant? Restaurant { get; set; }
}