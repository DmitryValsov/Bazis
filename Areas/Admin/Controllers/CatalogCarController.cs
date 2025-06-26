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

namespace Bazis.Areas.Admin.Controllers
{
    [Area("Admin")]
        public class CatalogCarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnv;

        public CatalogCarController(ApplicationDbContext context, IWebHostEnvironment hostEnv)
        {
            _context = context;
            _hostEnv = hostEnv;
        }

        // GET: CatalogCar
        public async Task<IActionResult> Index()
        {
            return View(await _context.CatalogCar.ToListAsync());
        }

        // GET: CatalogCar/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogCar = await _context.CatalogCar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (catalogCar == null)
            {
                return NotFound();
            }

            return View(catalogCar);
        }

        // GET: CatalogCar/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CatalogCar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,PreviewText,Link,ReleaseDate,ImageFile")] CatalogCar catalogCar)
        {
            if (ModelState.IsValid)
    {
        // Если файл выбран — сохраняем
        if (catalogCar.ImageFile is not null)
        {
            // Получаем путь к wwwroot/images
            string uploads = Path.Combine(_hostEnv.WebRootPath, "img_catalog");
            // Делаем уникальное имя
            string fileName = Path.GetFileNameWithoutExtension(catalogCar.ImageFile.FileName)
                              + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                              + Path.GetExtension(catalogCar.ImageFile.FileName);
            string fullPath = Path.Combine(uploads, fileName);

            // Сохраняем файл на диск
            using var fileStream = new FileStream(fullPath, FileMode.Create);
            await catalogCar.ImageFile.CopyToAsync(fileStream);

            // В БД сохраняем только имя файла
            catalogCar.Img = fileName;
        }

        _context.Add(catalogCar);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    return View(catalogCar);
        }

        // GET: CatalogCar/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogCar = await _context.CatalogCar.FindAsync(id);
            if (catalogCar == null)
            {
                return NotFound();
            }
            return View(catalogCar);
        }

        // POST: CatalogCar/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,PreviewText,ReleaseDate,Img")] CatalogCar catalogCar)
        {
            if (id != catalogCar.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catalogCar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatalogCarExists(catalogCar.Id))
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
            return View(catalogCar);
        }

        // GET: CatalogCar/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogCar = await _context.CatalogCar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (catalogCar == null)
            {
                return NotFound();
            }

            return View(catalogCar);
        }

        // POST: CatalogCar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var catalogCar = await _context.CatalogCar.FindAsync(id);
            if (catalogCar != null)
            {
                _context.CatalogCar.Remove(catalogCar);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CatalogCarExists(int id)
        {
            return _context.CatalogCar.Any(e => e.Id == id);
        }
    }
}
