using AdminPanel.Common.Interfaces;
using AdminPanel.Common.Models;
using Auth.DAL;
using Auth.DAL.Entities;
using Backend.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.BL.Services;

public class ProfileService : IProfileService
{
    private readonly AppDbContext _authDbContext;
    private readonly ApplicationDbContext _backendDbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileService(AppDbContext authDbContext, ApplicationDbContext backendDbContext,
        UserManager<ApplicationUser> userManager)
    {
        _authDbContext = authDbContext;
        _backendDbContext = backendDbContext;
        _userManager = userManager;
    }

    public async Task<List<User>> GetUsers()
    {
        var users = await _authDbContext
            .Users
            .Select(u => new User
            {
                Id = u.Id,
                UserName = u.FullName,
                Email = u.Email,
                BirthDate = u.BirthDate ?? DateTime.UtcNow,
                Gender = u.Gender == Gender.Male.ToString() ? Gender.Male : Gender.Female,
                PhoneNumber = u.PhoneNumber
            })
            .ToListAsync();

        foreach (var user in users)
        {
            user.Roles = await GetUserRoles(user.Id);
        }

        return users;
    }

    public async Task ChangeUser(User user)
    {
        var userEntity = await _authDbContext
            .Users
            .FirstOrDefaultAsync(u => u.Id == user.Id);
        await ValidateChange(user);
        if (userEntity == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "User was not found"
            );
            throw ex;
        }

        userEntity.Email = user.Email;
        userEntity.PhoneNumber = user.PhoneNumber;
        userEntity.BirthDate = user.BirthDate.ToUniversalTime();
        userEntity.UserName = user.Email;
        userEntity.FullName = user.UserName;
        userEntity.Gender = user.Gender.ToString();

        var rolesClone = new List<Role>(user.Roles);
        await ChangeUserRoles(userEntity, rolesClone, user.Address ?? "");

        if (user.Roles.Any(r => r.Name is Roles.Cook or Roles.Manager))
        {
            await AddRestaurantsToRoles(user);
        }

        await _authDbContext.SaveChangesAsync();
    }

    public async Task<User> GetUserInfo(Guid id)
    {
        var user = await _authDbContext
            .Users
            .Where(user => user.Id == id)
            .Select(u => new User
            {
                Id = u.Id,
                UserName = u.FullName,
                Email = u.Email,
                BirthDate = u.BirthDate ?? DateTime.UtcNow,
                Gender = u.Gender == Gender.Male.ToString() ? Gender.Male : Gender.Female,
                PhoneNumber = u.PhoneNumber
            }).FirstOrDefaultAsync();
        if (user == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "User was not found"
            );
            throw ex;
        }

        user.Roles = await GetUserRoles(user.Id);
        user.RestaurantIds = await GetRestaurants(user.Id);
        var selectedRest = user.RestaurantIds.FirstOrDefault(r => r.Selected);
        user.RestaurantId = selectedRest?.Value;

        var customer = await _backendDbContext
            .Users
            .FirstOrDefaultAsync(c => c.Id == user.Id);
        user.Address = customer?.Address;

        return user;
    }

    public async Task DeleteUser(Guid id)
    {
        var user = await _userManager
            .Users
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "User was not found"
            );
            throw ex;
        }

        var roles = await _userManager.GetRolesAsync(user);
        await DeleteRolesEntities(user);

        await _userManager.RemoveFromRolesAsync(user, roles);
        await _userManager.DeleteAsync(user);
    }

    private async Task<List<Role>> GetUserRoles(Guid userId)
    {
        var roles = await _authDbContext
            .Roles
            .Where(r => r.Name != "Admin")
            .ToListAsync();
        var userRolesId = await _authDbContext
            .UserRoles
            .Where(role => role.UserId == userId)
            .ToListAsync();

        return roles.Select(role => new Role
            {
                Id = role.Id,
                Name = role.Name == Roles.Customer.ToString() ? Roles.Customer :
                    role.Name == Roles.Courier.ToString() ? Roles.Courier :
                    role.Name == Roles.Manager.ToString() ? Roles.Manager : Roles.Cook,
                Selected = userRolesId.Any(r => r.RoleId == role.Id)
            })
            .ToList();
    }

    private async Task DeleteRolesEntities(ApplicationUser user)
    {
        if (user.Manager != null)
        {
            var managerAuth = await _authDbContext
                .Managers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.User == user);
            if (managerAuth != null)
            {
                _authDbContext.Managers.Remove(managerAuth);
            }

            var managerBack = await _backendDbContext
                .Managers
                .FirstOrDefaultAsync(m => m.Id == user.Manager.Id);
            if (managerBack != null)
            {
                _backendDbContext.Managers.Remove(managerBack);
            }
        }

        if (user.Courier != null)
        {
            var courierAuth = await _authDbContext
                .Couriers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User == user);
            if (courierAuth != null)
            {
                _authDbContext.Couriers.Remove(courierAuth);
            }

            var courierBack = await _backendDbContext
                .Couriers
                .FirstOrDefaultAsync(c => c.Id == user.Courier.Id);
            if (courierBack != null)
            {
                _backendDbContext.Couriers.Remove(courierBack);
            }
        }

        if (user.Customer != null)
        {
            var customerAuth = await _authDbContext
                .Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User == user);
            if (customerAuth != null)
            {
                _authDbContext.Customers.Remove(customerAuth);
            }

            var customerBack = await _backendDbContext
                .Users
                .FirstOrDefaultAsync(c => c.Id == user.Customer.Id);
            if (customerBack != null)
            {
                _backendDbContext.Users.Remove(customerBack);
            }
        }

        if (user.Cook != null)
        {
            var cookAuth = await _authDbContext
                .Cooks
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User == user);
            if (cookAuth != null)
            {
                _authDbContext.Cooks.Remove(cookAuth);
            }

            var cookBack = await _backendDbContext
                .Cooks
                .FirstOrDefaultAsync(c => c.Id == user.Cook.Id);
            if (cookBack != null)
            {
                _backendDbContext.Cooks.Remove(cookBack);
            }
        }

        await _authDbContext.SaveChangesAsync();
    }

    private async Task<List<SelectListItem>> GetRestaurants(Guid userId)
    {
        var restaurantEntities = await _backendDbContext
            .Restaurants
            .Include(r => r.Managers)
            .Include(r => r.Cooks)
            .ToListAsync();

        return restaurantEntities.Select(rest => new SelectListItem
        {
            Value = rest.Id.ToString(),
            Text = rest.Name,
            Selected = rest.Managers.Any(m => m.Id == userId) || rest.Cooks.Any(c => c.Id == userId)
        }).ToList();
    }

    private async Task ValidateChange(User editUser)
    {
        if (await _authDbContext.Users.AnyAsync(user =>
                user.Email == editUser.Email && user.Id != editUser.Id))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Duplicate user email"
            );
            throw ex;
        }

        if (DateTime.UtcNow.Year - editUser.BirthDate.Year is >= 0 and <= 200) return;

        var exc = new Exception();
        exc.Data.Add(StatusCodes.Status400BadRequest.ToString(),
            "Invalid Birth Date"
        );
        throw exc;
    }

    private async Task ChangeUserRoles(ApplicationUser user, List<Role> roles, string address)
    {
        var prevUserRoles = await _authDbContext
            .UserRoles
            .Where(r => r.UserId == user.Id)
            .ToListAsync();
        foreach (var prevRole in prevUserRoles.ToList())
        {
            var role = roles.FirstOrDefault(r => r.Id == prevRole.RoleId);
            if (role == null)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                    "Role was not found"
                );
                throw ex;
            }

            if (!role.Selected) continue;
            roles.Remove(role);
            prevUserRoles.Remove(prevRole);
        }

        await DeletePrevRoles(prevUserRoles, user);

        foreach (var role in roles)
        {
            if (prevUserRoles.All(r => r.RoleId != role.Id) && role.Selected)
            {
                switch (role.Name)
                {
                    case Roles.Customer:
                        var customerAuth = new Customer
                        {
                            Id = Guid.NewGuid(),
                            User = user,
                            Address = address
                        };
                        await _authDbContext.Customers.AddAsync(customerAuth);
                        var customerBack = new Backend.DAL.Entities.User
                        {
                            Id = user.Id,
                            Address = address
                        };
                        await _backendDbContext.Users.AddAsync(customerBack);
                        break;

                    case Roles.Cook:
                        var cookAuth = new Cook
                        {
                            Id = Guid.NewGuid(),
                            User = user
                        };
                        await _authDbContext.Cooks.AddAsync(cookAuth);
                        var cookBack = new Backend.DAL.Entities.Cook
                        {
                            Id = user.Id
                        };
                        await _backendDbContext.Cooks.AddAsync(cookBack);
                        break;

                    case Roles.Courier:
                        var courierAuth = new Courier
                        {
                            Id = Guid.NewGuid(),
                            User = user,
                            UserId = user.Id
                        };
                        await _authDbContext.Couriers.AddAsync(courierAuth);
                        var courierBack = new Backend.DAL.Entities.Courier
                        {
                            Id = user.Id,
                        };
                        await _backendDbContext.Couriers.AddAsync(courierBack);
                        break;

                    case Roles.Manager:
                        var managerAuth = new Manager
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            User = user
                        };
                        await _authDbContext.Managers.AddAsync(managerAuth);
                        var managerBack = new Backend.DAL.Entities.Manager
                        {
                            Id = user.Id,
                        };
                        await _backendDbContext.Managers.AddAsync(managerBack);
                        break;
                }

                await _authDbContext.SaveChangesAsync();
                await _backendDbContext.SaveChangesAsync();
                await _userManager.AddToRoleAsync(user, role.Name.ToString());
            }
        }
    }

    private async Task DeletePrevRoles(List<IdentityUserRole<Guid>> prevRoles, ApplicationUser user)
    {
        foreach (var role in prevRoles)
        {
            var appRole = await _authDbContext.Roles.FirstOrDefaultAsync(ar => ar.Id == role.RoleId);
            if (appRole == null)
            {
                continue;
            }

            var roleEnum = Enum.Parse(typeof(Roles), appRole.Name);
            switch (roleEnum)
            {
                case Roles.Cook:
                    var cookAuth = await _authDbContext
                        .Cooks
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(cook => cook.User.Id == user.Id);
                    var cookBack = await _backendDbContext
                        .Cooks
                        .FirstOrDefaultAsync(cook => cook.Id == user.Id);
                    if (cookBack != null)
                    {
                        _backendDbContext.Cooks.Remove(cookBack);
                    }

                    await _userManager.RemoveFromRoleAsync(user, Roles.Cook.ToString());
                    if (cookAuth != null)
                    {
                        _authDbContext.Cooks.Remove(cookAuth);
                    }

                    break;

                case Roles.Customer:
                    var customerAuth = await _authDbContext
                        .Customers
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.User.Id == user.Id);
                    var customerBack = await _backendDbContext
                        .Users
                        .FirstOrDefaultAsync(customer => customer.Id == user.Id);
                    if (customerBack != null)
                    {
                        _backendDbContext.Users.Remove(customerBack);
                    }

                    await _userManager.RemoveFromRoleAsync(user, Roles.Customer.ToString());
                    if (customerAuth != null)
                    {
                        _authDbContext.Customers.Remove(customerAuth);
                    }

                    break;

                case Roles.Courier:
                    var courierAuth = await _authDbContext
                        .Couriers
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.User.Id == user.Id);
                    var courierBack = await _backendDbContext
                        .Couriers
                        .FirstOrDefaultAsync(courier => courier.Id == user.Id);
                    if (courierBack != null)
                    {
                        _backendDbContext.Couriers.Remove(courierBack);
                    }

                    await _userManager.RemoveFromRoleAsync(user, Roles.Courier.ToString());
                    if (courierAuth != null)
                    {
                        _authDbContext.Couriers.Remove(courierAuth);
                    }

                    break;

                case Roles.Manager:
                    var managerAuth = await _authDbContext
                        .Managers
                        .Include(m => m.User)
                        .FirstOrDefaultAsync(m => m.User.Id == user.Id);
                    var managerBack = await _backendDbContext
                        .Managers
                        .FirstOrDefaultAsync(manager => manager.Id == user.Id);
                    if (managerBack != null)
                    {
                        _backendDbContext.Managers.Remove(managerBack);
                    }

                    await _userManager.RemoveFromRoleAsync(user, Roles.Manager.ToString());
                    if (managerAuth != null)
                    {
                        _authDbContext.Managers.Remove(managerAuth);
                    }

                    break;
            }

            await _backendDbContext.SaveChangesAsync();
            await _authDbContext.SaveChangesAsync();
        }
    }

    private async Task AddRestaurantsToRoles(User editUser)
    {
        var restaurant = await _backendDbContext
            .Restaurants
            .FirstOrDefaultAsync(r => r.Id.ToString() == editUser.RestaurantId);
        if (editUser.Roles.Any(r => r.Name == Roles.Manager && r.Selected))
        {
            var manager = await _backendDbContext
                .Managers
                .FirstOrDefaultAsync(m => m.Id == editUser.Id);
            if (manager != null)
            {
                manager.Restaurant = restaurant;
            }
        }

        if (editUser.Roles.Any(r => r is { Name: Roles.Cook, Selected: true }))
        {
            var cook = await _backendDbContext
                .Cooks
                .FirstOrDefaultAsync(c => c.Id == editUser.Id);
            if (cook != null)
            {
                cook.Restaurant = restaurant;
            }
        }

        await _backendDbContext.SaveChangesAsync();
    }
}