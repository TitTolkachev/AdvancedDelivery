using Microsoft.AspNetCore.Authorization;

namespace DeliveryBackend.Services.ValidateTokenPolicy;

public class ValidateTokenRequirement : IAuthorizationRequirement
{
    public ValidateTokenRequirement()
    {
    }
}