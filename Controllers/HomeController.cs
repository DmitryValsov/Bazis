using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bazis.Models;
using Microsoft.AspNetCore.Authorization;
using Bazis.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace Bazis.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;


    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }



    //[Authorize(Roles = "Administrator")]
    [Authorize]
    //public IActionResult Index()
    //{
    //   return View();
    //}

    public async Task<IActionResult> Index()
    {

        var user = await _userManager.GetUserAsync(User);

        ViewBag.News = await _context.News.ToListAsync();
        ViewBag.CatalogCar = await _context.CatalogCar.ToListAsync();


        ViewBag.FinishOrders = await _context.Orders
            .Where(p => p.UserId == user.Id)
            .Where(c => c.Status == "1")
            .OrderByDescending(p => p.CreatedAt)
            .Take(3)
            .ToListAsync();

        ViewBag.ActiveOrders = await _context.Orders
            .Where(p => p.UserId == user.Id)
            .Where(c => c.Status == "0")
             .OrderByDescending(p => p.CreatedAt)
            .Take(3)
            .ToListAsync();



        ViewBag.LastOrder = await _context.Orders
            .OrderByDescending(e => e.CreatedAt) // Замените `Id` на актуальный столбец
            .FirstOrDefaultAsync();

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
