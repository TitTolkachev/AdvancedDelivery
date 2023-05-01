using AdminPanel.Common.Interfaces;
using AdminPanel.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.MVC.Controllers;

public class RestaurantController : Controller
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    public async Task<IActionResult> Index()
    {
        var restaurants = await _restaurantService.GetRestaurants();

        return View(restaurants);
    }

    [HttpPost]
    public IActionResult Create(Restaurant restaurant)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        try
        {
            // TODO(Добавить ресторан)
            
            TempData["msg"] = "Restaurant created successfully!";
            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            TempData["msg"] = "FAILED! Restaurant was not created!";
            return View();
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Change()
    {
        return View();
    }
}