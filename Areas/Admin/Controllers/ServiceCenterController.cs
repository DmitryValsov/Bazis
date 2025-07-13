using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bazis.Data;
using Bazis.Models;

namespace Bazis.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceCenterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceCenterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ServiceCenter
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServiceCenters.ToListAsync());
        }

        // GET: ServiceCenter/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceCenter = await _context.ServiceCenters
                .FirstOrDefaultAsync(m => m.ServiceCenterId == id);
            if (serviceCenter == null)
            {
                return NotFound();
            }

            return View(serviceCenter);
        }

        // GET: ServiceCenter/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServiceCenter/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceCenterId,Name,Address,Phone,OpeningHours,MapLink,Rating")] ServiceCenter serviceCenter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceCenter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceCenter);
        }

        // GET: ServiceCenter/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceCenter = await _context.ServiceCenters.FindAsync(id);
            if (serviceCenter == null)
            {
                return NotFound();
            }
            return View(serviceCenter);
        }

        // POST: ServiceCenter/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceCenterId,Name,Address,Phone,OpeningHours,MapLink,Rating")] ServiceCenter serviceCenter)
        {
            if (id != serviceCenter.ServiceCenterId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceCenter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceCenterExists(serviceCenter.ServiceCenterId))
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
            return View(serviceCenter);
        }

        // GET: ServiceCenter/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceCenter = await _context.ServiceCenters
                .FirstOrDefaultAsync(m => m.ServiceCenterId == id);
            if (serviceCenter == null)
            {
                return NotFound();
            }

            return View(serviceCenter);
        }

        // POST: ServiceCenter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceCenter = await _context.ServiceCenters.FindAsync(id);
            if (serviceCenter != null)
            {
                _context.ServiceCenters.Remove(serviceCenter);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceCenterExists(int id)
        {
            return _context.ServiceCenters.Any(e => e.ServiceCenterId == id);
        }
    }
}
