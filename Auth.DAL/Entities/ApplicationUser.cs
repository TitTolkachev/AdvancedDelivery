using Microsoft.AspNetCore.Identity;

namespace Auth.DAL.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public Customer? Customer { get; set; }

    public Manager? Manager { get; set; }

    public Courier? Courier { get; set; }

    public Cook? Cook { get; set; }
}