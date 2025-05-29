using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bazis.Models;
using Microsoft.AspNetCore.Authorization;

namespace Bazis.Controllers;

public class OrderController : Controller
{
    private readonly ILogger<OrderController> _logger;

    public OrderController(ILogger<OrderController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    public IActionResult Index()
    {
        return View();
    }
}
