using System.ComponentModel.DataAnnotations;

namespace Auth.DAL.Entities;

public class Courier
{
    public Guid Id { get; set; }

    [Required] public ApplicationUser User { get; set; } = null!;
}