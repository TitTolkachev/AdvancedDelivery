using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.DAL.Entities;

public class Customer
{
    public Guid Id { get; set; }

    public string? Address { get; set; }

    [Required] public long UserId { get; set; }
    [Required] [ForeignKey("UserId")] public ApplicationUser User { get; set; } = null!;
}