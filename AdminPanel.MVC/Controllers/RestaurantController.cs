using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.MVC.Controllers;

public class RestaurantController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}