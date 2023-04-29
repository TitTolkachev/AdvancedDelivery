using Microsoft.AspNetCore.Authorization;

namespace Backend.BL.Services.ValidateTokenPolicy;

public class ValidateTokenRequirement : IAuthorizationRequirement
{
    public ValidateTokenRequirement()
    {
    }
}