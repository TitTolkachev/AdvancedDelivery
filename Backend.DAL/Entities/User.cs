using System.ComponentModel.DataAnnotations;

namespace Backend.DAL.Entities;

public class User
{
    public Guid Id { get; set; }
    [Required]
    [MinLength(1)]
    public string FullName { get; set; } = null!;

    public DateTime? BirthDate { get; set; }
    [Required]
    public string Gender { get; set; } = null!;

    [Phone]
    public string? PhoneNumber { get; set; }
    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public string? Address { get; set; }
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;

    public List<Rating> Ratings { get; set; } = null!;
    public List<Order> Orders { get; set; } = null!;
    public List<Cart> Carts { get; set; } = null!;
}