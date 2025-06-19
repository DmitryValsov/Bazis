using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bazis.Models;
using Microsoft.AspNetCore.Authorization;

namespace Bazis.Controllers;

public class CarController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public CarController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    //[Authorize(Roles = "Administrator")]
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
