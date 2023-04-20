using Auth.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace Auth.BL.Services;

public interface ITokenService
{
    string CreateToken(ApplicationUser user, List<IdentityRole<long>> role);
}