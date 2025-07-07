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
    public class QuotationsController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Quotations
        public async Task<IActionResult> Index()
        {
            var quotations = await _context.Quotation
                .Include(q => q.ExpenseHead)
                .Include(q => q.Employee)
                .ToListAsync();

            return View(quotations);
        }

        public async Task<IActionResult> Upsert(int? id, bool isReadOnly = false)
        {
            ViewBag.ExpenseHeadID = new SelectList(await _context.ExpenseHead.ToListAsync(), "Id", "ExpenseName");
            ViewBag.ItemCategoryId = new SelectList(await _context.ItemCategories.ToListAsync(), "Id", "CategoryName");
            ViewBag.EmployeeList = new SelectList(await _context.Employee.ToListAsync(), "Id", "Name");
            ViewBag.CostBaseOptions = new SelectList(new[] { "Per Consignment", "Per Container" });
            ViewBag.DeliveryType = new SelectList(new[] { "On Chassis", "Open Delivery", "Partial On Chassis" });
            ViewBag.IsReadOnly = isReadOnly;

            if (id == null || id == 0)
            {
                return View(new Quotation());
            }
            else
            {
                var quotation = await _context.Quotation.FindAsync(id);
                if (quotation == null)
                {
                    return NotFound();
                }
                return View(quotation);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Quotation quotation)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ExpenseHeadID = new SelectList(await _context.ExpenseHead.ToListAsync(), "Id", "ExpenseName");
                ViewBag.ItemCategoryId = new SelectList(await _context.ItemCategories.ToListAsync(), "Id", "CategoryName");
                return View(quotation);
            }

            if (quotation.Id == 0)
            {
                _context.Quotation.Add(quotation);
            }
            else
            {
                _context.Quotation.Update(quotation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
