using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClearingAndForwarding.Context;
using ClearingAndForwarding.Models;

namespace ClearingAndForwarding.Controllers
{
    public class UnitNamesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UnitNamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UnitNames
        public async Task<IActionResult> Index()
        {
            return View(await _context.UnitName.ToListAsync());
        }

        // GET: UnitNames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitName = await _context.UnitName
                .FirstOrDefaultAsync(m => m.Id == id);
            if (unitName == null)
            {
                return NotFound();
            }

            return View(unitName);
        }

        // GET: UnitNames/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UnitNames/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CodeName,CompanyName,BINno,TINno,Address,Type")] UnitName unitName)
        {
            if (ModelState.IsValid)
            {
                _context.Add(unitName);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(unitName);
        }

        // GET: UnitNames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitName = await _context.UnitName.FindAsync(id);
            if (unitName == null)
            {
                return NotFound();
            }
            return View(unitName);
        }

        // POST: UnitNames/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodeName,CompanyName,BINno,TINno,Address,Type")] UnitName unitName)
        {
            if (id != unitName.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(unitName);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnitNameExists(unitName.Id))
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
            return View(unitName);
        }

        // GET: UnitNames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitName = await _context.UnitName
                .FirstOrDefaultAsync(m => m.Id == id);
            if (unitName == null)
            {
                return NotFound();
            }

            return View(unitName);
        }

        // POST: UnitNames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var unitName = await _context.UnitName.FindAsync(id);
            if (unitName != null)
            {
                _context.UnitName.Remove(unitName);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UnitNameExists(int id)
        {
            return _context.UnitName.Any(e => e.Id == id);
        }
    }
}