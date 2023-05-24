using AdminPanel.Common.Interfaces;
using AdminPanel.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.MVC.Controllers;

public class ProfileController : Controller
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _profileService.GetUsers();
        return View(users);
    }

    public async Task<IActionResult> Change(Guid id)
    {
        var user = await _profileService.GetUserInfo(id);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Change(User user)
    {
        if (!ModelState.IsValid)
        {
            // var userInfo = _mapper.Map<UserInfo>(user);
            // return View(userInfo);
            return View(user);
        }

        try
        {
            await _profileService.ChangeUser(user);
        }
        catch (Exception)
        {
            // ignored
        }

        if (!ModelState.IsValid)
        {
            // var userInfo = _mapper.Map<UserInfo>(user);
            // return View(userInfo);
            return View(user);
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        await _profileService.DeleteUser(id);
        return RedirectToAction("Index");
    }
}