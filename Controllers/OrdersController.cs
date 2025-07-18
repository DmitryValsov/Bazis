using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bazis.Data;
using Bazis.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Bazis.Services;
using System.Runtime.CompilerServices;

namespace Bazis.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        private readonly UserManager<IdentityUser> _userManager;
            private readonly TelegramService _telegramService;



        public OrdersController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<IdentityUser> userManager,TelegramService telegramService)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _telegramService = telegramService;

        }

        // GET: Orders

        public async Task<IActionResult> DashBoard()
        {
        
             var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(); // Или перенаправить на страницу входа
            }

            var applicationDbContext = await _context.Orders
            .Where(p => p.UserId == user.Id) // Предполагается, что у вас есть поле UserId в модели Post
            .ToListAsync();

            //var applicationDbContext = _context.Orders.Include(o => o.UserId);
            //return View(await applicationDbContext.ToListAsync());
            return View(applicationDbContext);
        }


        public async Task<IActionResult> Index()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(); // Или перенаправить на страницу входа
            }

            
            ViewBag.FinishOrders = await _context.Orders
            .Where(p => p.UserId == user.Id)
            .Where(c => c.Status == "1")
            .Include(c => c.Car)
            .OrderByDescending(p => p.Id)
            .ToListAsync();

            ViewBag.ActiveOrders = await _context.Orders
            .Where(p => p.UserId == user.Id)
            .Where(c => c.Status == "0")
            .Include(c => c.Car)
            .OrderByDescending(p => p.Id)
            .ToListAsync();

            //ViewBag.CatalogCar = await _context.CatalogCar.ToListAsync();

            return View();
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Car)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {

            //ViewData["UserIdCur"] = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["UserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var currentTime = DateTime.Now;
            ViewData["CreatedAt"] = currentTime.ToString();


            var userId = _userManager.GetUserId(User);
            ViewBag.UsersCars = _context.Car.Where(p => p.UserId == userId).ToList();

            ViewBag.Services = _context.ServiceCenters.ToList();

             ViewBag.Types = _context.ServiceTypes.ToList();

           // ViewData["CreatedAt"] = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //ViewData["UserId"] = _context.GetUserId(User);
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatedAt,CarId,UserId,Name,Phone,ServiceAddress,Usluga,Comment,Status,Date,Time")] Order order)
        {
            if (ModelState.IsValid)
            {


                string telegramMessage = $"Получена новая запись через сервис";

                await _telegramService.SendMessageAsync(telegramMessage);


                _context.Add(order);
                await _context.SaveChangesAsync();


                //   string subject = "Подтверждение бронирования";
           // Отправляем сообщение в Telegram администратору

           

           

                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);


            string tg = "Проверка";
           await _telegramService.SendMessageAsync(tg);

            //ViewData["UserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreatedAt,UserId,Name,Phone,ServiceAddress,Usluga,Comment,Status,Date,Time")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
