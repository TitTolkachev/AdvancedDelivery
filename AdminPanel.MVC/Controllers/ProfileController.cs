using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.MVC.Controllers;

public class ProfileController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}