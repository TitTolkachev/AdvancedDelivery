using System.IdentityModel.Tokens.Jwt;
using Auth.Common.Dto;
using Auth.Common.Extensions;
using Auth.Common.Interfaces;
using Auth.DAL;
using Auth.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Auth.BL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;

    public AuthService(ITokenService tokenService, AppDbContext context, UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Login(AuthRequest request)
    {
        var managedUser = await _userManager.FindByEmailAsync(request.Email);

        if (managedUser == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Bad credentials"
            );
            throw ex;
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);

        if (!isPasswordValid)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Bad credentials"
            );
            throw ex;
        }

        var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

        if (user is null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(), "Unauthorized");
            throw ex;
        }

        var roleIds = await _context.UserRoles.Where(r => r.UserId == user.Id).Select(x => x.RoleId).ToListAsync();
        var roles = _context.Roles.Where(x => roleIds.Contains(x.Id)).ToList();

        var accessToken = _tokenService.CreateToken(user, roles);
        user.RefreshToken = _configuration.GenerateRefreshToken();
        user.RefreshTokenExpiryTime =
            DateTime.UtcNow.AddDays(_configuration.GetSection("Jwt:RefreshTokenValidityInDays").Get<int>());

        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Username = user.FullName,
            Email = user.Email!,
            Token = accessToken,
            RefreshToken = user.RefreshToken
        };
    }

    public async Task<AuthResponse> Register(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            FullName = request.FullName,
            BirthDate = request.BirthDate,
            Gender = request.Gender,
            PhoneNumber = request.Phone,
            Address = request.Address,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Errors.Any())
        {
            var exc = new Exception();
            exc.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Bad credentials"
            );
            throw exc;
        }

        var findUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        if (findUser == null) throw new Exception($"User {request.Email} not found");

        // По умолчанию роль Customer
        await _userManager.AddToRoleAsync(findUser, Roles.Customer);

        return await Login(new AuthRequest
        {
            Email = request.Email,
            Password = request.Password
        });
    }

    public async Task Revoke(string userEmail)
    {
        var user = await _userManager.FindByNameAsync(userEmail);
        if (user == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Invalid user name");
            throw ex;
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(user);
    }

    public async Task<Token> RefreshToken(Token tokenModel)
    {
        var accessToken = tokenModel.AccessToken;
        var refreshToken = tokenModel.RefreshToken;
        var principal = _configuration.GetPrincipalFromExpiredToken(accessToken);

        if (principal == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Bad Request");
            throw ex;
        }

        var username = principal.Identity!.Name;
        var user = await _userManager.FindByNameAsync(username!);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(), "Invalid access token or refresh token");
            throw ex;
        }

        var newAccessToken = _configuration.CreateToken(principal.Claims.ToList());
        var newRefreshToken = _configuration.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new Token
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken
        };
    }
}