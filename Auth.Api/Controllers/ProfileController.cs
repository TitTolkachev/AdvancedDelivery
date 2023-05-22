using System.Security.Claims;
using Auth.Common.Dto;
using Auth.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Auth.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get user profile")]
    public async Task<ActionResult<ProfileResponse>> GetUserProfile()
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        if (userEmail == null) return BadRequest("Invalid user name");
        return Ok(await _profileService.GetUserProfile(userEmail));
    }

    [HttpPut]
    [SwaggerOperation(Summary = "Change user profile")]
    public async Task<ActionResult> ChangeUserProfile([FromBody] ProfileRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        if (userEmail == null) return Unauthorized("Unauthorized");
        await _profileService.ChangeUserProfile(request, userEmail);

        return Ok();
    }
    
    [HttpDelete]
    [SwaggerOperation(Summary = "Delete user profile")]
    public async Task<ActionResult> DeleteProfile()
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        if (userEmail == null) return Unauthorized("Unauthorized");
        await _profileService.DeleteUserProfile(userEmail);

        return Ok();
    }
    
    [HttpPut]
    [Route("password")]
    [SwaggerOperation(Summary = "Change user password (requires old and new passwords)")]
    public async Task<ActionResult> ChangeUserPassword([FromBody] PasswordChange passwords)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        if (userEmail == null) return Unauthorized("Unauthorized");
        await _profileService.ChangeUserPassword(passwords, userEmail);

        return Ok();
    }
}