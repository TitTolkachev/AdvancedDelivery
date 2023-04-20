using Microsoft.AspNetCore.Identity;

namespace Auth.DAL.Entities;

public class ApplicationUser : IdentityUser<long>
{
    public string FullName { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}