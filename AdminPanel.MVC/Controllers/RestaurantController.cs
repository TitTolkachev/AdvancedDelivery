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
    public async Task<IActionResult> Create(Restaurant restaurant)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        try
        {
            await _restaurantService.CreateRestaurant(restaurant);

            TempData["msg"] = "Restaurant created successfully!";
            return RedirectToAction("Create");
        }
        catch (Exception)
        {
            TempData["msg"] = "FAILED! Restaurant was not created!";
            return View();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Change(Guid id, Restaurant restaurant)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        try
        {
            await _restaurantService.ChangeRestaurant(restaurant);

            TempData["msg"] = "Restaurant changed successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            TempData["msg"] = "FAILED! Restaurant was not changed!";
            return View();
        }
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _restaurantService.DeleteRestaurant(id);
        }
        catch (Exception)
        {
            // ignored
        }

        return RedirectToAction("Index");
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