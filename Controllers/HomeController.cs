using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bazis.Models;
using Microsoft.AspNetCore.Authorization;
using Bazis.Data;
using Microsoft.EntityFrameworkCore;



namespace Bazis.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;


    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }



    //[Authorize(Roles = "Administrator")]
    [Authorize]
    //public IActionResult Index()
    //{
    //   return View();
    //}

    public async Task<IActionResult> Index()
    {
        ViewBag.News = await _context.News.ToListAsync();
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
