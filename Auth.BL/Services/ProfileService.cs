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

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;

    public ProfileService(ITokenService tokenService, AppDbContext context, UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> GetUserProfile()
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
            Username = user.UserName!,
            Email = user.Email!,
            Token = accessToken,
            RefreshToken = user.RefreshToken
        };
    }

    public async Task<AuthResponse> ChangeUserProfile(RegisterRequest request)
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
}