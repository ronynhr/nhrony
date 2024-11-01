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
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExpensesController> _logger; // Add this line

        public ExpensesController(ApplicationDbContext context, ILogger<ExpensesController> logger)
        {
            _context = context;
            _logger = logger; // Assign the logger
        }



        public async Task<IActionResult> Duplicate()
        {
            // Group expenses by BEno, FileNo, and BillDate to find duplicates
            var duplicateExpenses = await _context.Expenses
                .GroupBy(e => new { e.BEno, e.FileNo, e.BillAmount }) // Group by unique fields
                .Where(g => g.Count() > 1) // Only keep groups with more than one item
                .Select(g => new
                {
                    BEno = g.Key.BEno,
                    FileNo = g.Key.FileNo,
                    BillDate = g.Key.BillAmount,
                    Count = g.Count(),
                    Expenses = g.ToList() // Get the list of duplicate expenses
                })
                .ToListAsync();

            return View(duplicateExpenses);
        }





        // GET: Expenses
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var pageSize = 10; // Set your page size
            var expenses = await _context.Expenses
                .Include(e => e.Employee) // This ensures the Employee details are included
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalExpenses = await _context.Expenses.CountAsync();

            var model = new ExpensesViewModel
            {
                Expenses = expenses,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalExpenses / pageSize)
            };

            return View(model);
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenses = await _context.Expenses
                .Include(e => e.ExpenseDetails) // Ensure this is correct
                .ThenInclude(ed => ed.ExpenseHead) // Include related ExpenseHead
                .Include(e => e.Employee) // Include the Employee details
                .FirstOrDefaultAsync(m => m.Id == id);

            if (expenses == null)
            {
                return NotFound();
            }

            return View(expenses);
        }


        // POST: Expenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BEno,FileNo,BillType,BillAmount,BillDate,BillNo,PostingDate,LastEdit,EmployeeId")] Expenses expenses)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expenses);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(expenses);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenses = await _context.Expenses.FindAsync(id);
            if (expenses == null)
            {
                return NotFound();
            }
            return View(expenses);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BEno,FileNo,BillType,BillAmount,BillDate,BillNo,PostingDate,LastEdit,EmployeeId")] Expenses expenses)
        {
            if (id != expenses.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expenses);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpensesExists(expenses.Id))
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
            return View(expenses);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenses = await _context.Expenses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expenses == null)
            {
                return NotFound();
            }

            return View(expenses);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expenses = await _context.Expenses.FindAsync(id);
            if (expenses != null)
            {
                _context.Expenses.Remove(expenses);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpensesExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }
    }
}
