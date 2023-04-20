using Auth.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace Auth.Common.Interfaces;

public interface ITokenService
{
    string CreateToken(ApplicationUser user, List<IdentityRole<long>> role);
}