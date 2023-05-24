namespace Backend.Common.Dto;

public class MenuDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public List<Guid> Dishes { get; set; } = new(); 
}