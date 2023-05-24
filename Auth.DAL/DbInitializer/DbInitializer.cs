using Auth.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Auth.DAL.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly AppDbContext _context;

    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public DbInitializer(AppDbContext context, RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
    }

    public void Initialize()
    {
        try
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }
        }
        catch (Exception)
        {
            // ignored
        }

        if (_roleManager.RoleExistsAsync("Customer").GetAwaiter().GetResult()) return;

        _roleManager.CreateAsync(new IdentityRole<Guid>("Customer")).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole<Guid>("Courier")).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole<Guid>("Manager")).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole<Guid>("Cook")).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole<Guid>("Admin")).GetAwaiter().GetResult();

        _userManager.CreateAsync(new ApplicationUser
        {
            UserName = _configuration.GetSection("Admin:Name").Get<string>(),
            FullName = _configuration.GetSection("Admin:Name").Get<string>(),
            Email = _configuration.GetSection("Admin:Email").Get<string>(),
            EmailConfirmed = true
        }, _configuration.GetSection("Admin:Password").Get<string>()).GetAwaiter().GetResult();
        
        var user =
            _context.Users.FirstOrDefault(u => u.Email == _configuration.GetSection("Admin:Email").Get<string>());
        if (user != null) _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
    }
}