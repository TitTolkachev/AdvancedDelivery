using DeliveryBackend.DTO;
using DeliveryBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryBackend.Controllers;

[ApiController]
[Route("api/account")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost]
    [Route("register")]
    [SwaggerOperation(Summary = "Register new user")]
    public async Task<TokenResponse> RegisterUser([FromBody] UserRegisterModel userRegisterDto)
    {
        return await _usersService.RegisterUser(userRegisterDto);
    }

    [HttpPost]
    [Route("login")]
    [SwaggerOperation(Summary = "Log in to the system")]
    public async Task<TokenResponse> Login([FromBody] LoginCredentials credentials)
    {
        return await _usersService.LoginUser(credentials);
    }

    [HttpPost]
    [Route("logout")]
    [SwaggerOperation(Summary = "Log out system user")]
    public async Task Logout()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
        {
            throw new Exception("Token not found");
        }

        await _usersService.Logout(token);
    }

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("profile")]
    [SwaggerOperation(Summary = "Get user profile")]
    public async Task<UserDto> GetUserProfile()
    {
        return await _usersService.GetUserProfile(
            Guid.Parse(User.Identity.Name));
    }

    [HttpPut]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("profile")]
    [SwaggerOperation(Summary = "Edit user Profile")]
    public async Task EditUserProfile([FromBody] UserEditModel userEditModel)
    {
        await _usersService.EditUserProfile(
            Guid.Parse(User.Identity.Name), userEditModel);
    }
}