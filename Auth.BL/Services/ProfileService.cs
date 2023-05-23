using Auth.Common.Dto;
using Auth.Common.Interfaces;
using Auth.DAL;
using Auth.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.BL.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public ProfileService(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ProfileResponse> GetUserProfile(string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        var customer = await _context.Customers.FirstOrDefaultAsync(u => u.User == user);

        if (user is not null)
            return new ProfileResponse
            {
                FullName = user.Email,
                Address = customer?.Address,
                BirthDate = user.BirthDate,
                Email = user.Email,
                Gender = user.Gender,
                Phone = user.PhoneNumber
            };

        var ex = new Exception();
        ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
            "User not found"
        );
        throw ex;
    }

    public async Task ChangeUserProfile(ProfileRequest request, string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null)
        {
            var exc = new Exception();
            exc.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                $"User with Email {userEmail} not found"
            );
            throw exc;
        }

        user.FullName = request.FullName;
        user.PhoneNumber = request.Phone;
        if (user.Customer != null)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.User == user)!;
            customer.Address = request.Address;
            await _context.SaveChangesAsync();
        }

        var result = await _userManager.UpdateAsync(user);

        if (result.Errors.Any())
        {
            var exc = new Exception();
            exc.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                result.Errors.First().Description
            );
            throw exc;
        }
    }

    public async Task DeleteUserProfile(string userEmail)
    {
        var findUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

        if (findUser == null)
        {
            var exc = new Exception();
            exc.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                $"User with Email {userEmail} not found"
            );
            throw exc;
        }

        await _userManager.DeleteAsync(findUser);
    }

    public async Task ChangeUserPassword(PasswordChange passwords, string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null)
        {
            var exc = new Exception();
            exc.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                $"User with Email {userEmail} not found"
            );
            throw exc;
        }

        var result = await _userManager.ChangePasswordAsync(user, passwords.CurrentPassword, passwords.NewPassword);

        if (result.Errors.Any())
        {
            var exc = new Exception();
            exc.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                result.Errors.First().Description
            );
            throw exc;
        }
    }
}