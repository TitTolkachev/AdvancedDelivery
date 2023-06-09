﻿using System.Security.Claims;
using Auth.Common.Dto;
using Auth.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AccountsController : ControllerBase
{
    private readonly IAuthService _authService;

    public AccountsController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "User login")]
    public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        return Ok(await _authService.Login(request));
    }

    [HttpPost("register")]
    [SwaggerOperation(Summary = "User register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        return Ok(await _authService.Register(request));
    }

    [HttpPost("refresh")]
    [SwaggerOperation(Summary = "Refresh token (returns new access and refresh tokens)")]
    public async Task<ActionResult<Token>> RefreshToken(Token? tokenModel)
    {
        if (tokenModel is null) return BadRequest("Invalid client request");
        return Ok(await _authService.RefreshToken(tokenModel));
    }

    [Authorize]
    [HttpPost("revoke")]
    [SwaggerOperation(Summary = "Disable user refresh token")]
    public async Task<ActionResult> Revoke()
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        if (userEmail == null) return BadRequest("Invalid user name");
        await _authService.Revoke(userEmail);
        return Ok();
    }
}