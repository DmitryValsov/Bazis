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
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
         private readonly IWebHostEnvironment _hostEnv;

        public NewsController(ApplicationDbContext context, IWebHostEnvironment hostEnv)
        {
            _context = context;
              _hostEnv = hostEnv;
        }

        // GET: Admin/News

        public async Task<IActionResult> Index()
        {
            return View(await _context.News.ToListAsync());
        }

        // GET: Admin/News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: Admin/News/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,PreviewText,DetailText,NamePicture,Path,ReleaseDate,ImageFile")] News news)
        {


            if (ModelState.IsValid)
    {
        // Если файл выбран — сохраняем
        if (news.ImageFile is not null)
        {
            // Получаем путь к wwwroot/images
            string uploads = Path.Combine(_hostEnv.WebRootPath, "images");
            // Делаем уникальное имя
            string fileName = Path.GetFileNameWithoutExtension(news.ImageFile.FileName)
                              + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                              + Path.GetExtension(news.ImageFile.FileName);
            string fullPath = Path.Combine(uploads, fileName);

            // Сохраняем файл на диск
            using var fileStream = new FileStream(fullPath, FileMode.Create);
            await news.ImageFile.CopyToAsync(fileStream);

            // В БД сохраняем только имя файла
            news.Img = fileName;
        }

        _context.Add(news);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    return View(news);

            /*if (ModelState.IsValid)
            {
                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(news);
            */
        }

        // GET: Admin/News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: Admin/News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,PreviewText,DetailText,NamePicture,Path,ReleaseDate")] News news)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
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
            return View(news);
        }

        // GET: Admin/News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: Admin/News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}
