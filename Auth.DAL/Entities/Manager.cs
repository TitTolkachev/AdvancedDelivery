using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.DAL.Entities;

public class Manager
{
    public Guid Id { get; set; }

    [Required] public long UserId { get; set; }
    [Required] [ForeignKey("UserId")] public ApplicationUser User { get; set; } = null!;
}