using System.ComponentModel.DataAnnotations;

namespace Auth.DAL.Entities;

public class Customer
{
    public Guid Id { get; set; }

    public string? Address { get; set; }

    [Required] public ApplicationUser User { get; init; } = null!;
}