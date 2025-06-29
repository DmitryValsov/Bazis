
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
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Bazis.Controllers
{
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnv;
         private readonly UserManager<IdentityUser> _userManager;

        public CarController(ApplicationDbContext context, IWebHostEnvironment hostEnv, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _hostEnv = hostEnv;
            _userManager = userManager;
        }

        // GET: Car
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(); // Или перенаправить на страницу входа
            }

            var applicationDbContext = await _context.Car
            .Where(p => p.UserId == user.Id) 
            // Предполагается, что у вас есть поле UserId в модели Post
            .ToListAsync();

            return View(applicationDbContext);
        }

        // GET: Car/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Car/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return View();
        }

        // POST: Car/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Id,Brand,Model,Year,Color,VIN,GosNomer,ReleaseDate,ImageFile,UserId")] Car car)
{
    if (ModelState.IsValid)
    {
        // Если файл выбран — сохраняем
        if (car.ImageFile is not null)
        {
            // Получаем путь к wwwroot/images
            string uploads = Path.Combine(_hostEnv.WebRootPath, "images");
            // Делаем уникальное имя
            string fileName = Path.GetFileNameWithoutExtension(car.ImageFile.FileName)
                              + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                              + Path.GetExtension(car.ImageFile.FileName);
            string fullPath = Path.Combine(uploads, fileName);

            // Сохраняем файл на диск
            using var fileStream = new FileStream(fullPath, FileMode.Create);
            await car.ImageFile.CopyToAsync(fileStream);

            // В БД сохраняем только имя файла
            car.Img = fileName;
        }

        _context.Add(car);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", car.UserId);
    return View(car);
}


        // GET: Car/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Car.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
             ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", car.UserId);
            return View(car);
        }

        // POST: Car/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,Year,Color,VIN,GosNomer,Img,ReleaseDate")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
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
             ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", car.UserId);
            return View(car);
        }

        // GET: Car/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Car/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Car.FindAsync(id);
            if (car != null)
            {
                _context.Car.Remove(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Car.Any(e => e.Id == id);
        }
    }
}
