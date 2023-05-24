using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Common.Models.Restaurant;

public class Restaurant
{
    public Guid Id { get; set; }
    [Required] [MinLength(1)] public string Name { get; set; } = null!;
}