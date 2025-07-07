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
    public class RequisitionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequisitionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Requisitions
        public IActionResult Index(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Requisition
                .Include(r => r.Employee)
                .Include(r => r.Bedetails)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(r =>
                    EF.Functions.Like(r.Employee.Name, $"%{searchString}%") ||
                    EF.Functions.Like(r.Bedetails.BENo, $"%{searchString}%") ||
                    EF.Functions.Like(r.RequisitionNo, $"%{searchString}%") ||
                    EF.Functions.Like(r.RequisitionAmount.ToString(), $"%{searchString}%")
                );
            }

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var requisitions = query
                .OrderBy(r => r.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new RequisitionViewModel
            {
                SearchString = searchString,
                Requisitions = requisitions,
                PageNumber = pageNumber,
                TotalPages = totalPages
            };

            ViewBag.beList = _context.BEdetails
                .Select(b => new { value = b.Id, text = b.BENo })
                .ToDictionary(b => b.value, b => b.text);

            return View(model);
        }


        // GET: Requisitions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            

            var requisition = await _context.Requisition
                .Include(e => e.RequisitionDetails)
                    .ThenInclude(ed => ed.ExpenseHead)
                .Include(e => e.Employee)
                .Include(e => e.Bedetails)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (requisition == null)
            {
                return NotFound();
            }

            var expenseId = await _context.Requisition
                .Where(r => r.Id == id)
                .Select(r => r.ExpenseID)
                .FirstOrDefaultAsync();

            var matchingRequisitions = await _context.Requisition
                .Include(r => r.Expenses)
                .Include(r => r.Employee)
                .Include(r => r.RequisitionDetails)
                .Where(r => r.ExpenseID == expenseId && r.Id != id)
                .ToListAsync();

            ViewData["MatchingRequisitions"] = matchingRequisitions;

            var quotationAmounts = await _context.Quotation
                .Include(ed => ed.ExpenseHead)
                .Where(q => requisition.RequisitionDetails.Select(ed => ed.ExpenseHeadId).Contains(q.ExpenseHeadID)
                            && requisition.RequisitionDate >= q.EffectiveDate
                            && requisition.RequisitionDate <= (q.ExpiryDate ?? DateOnly.FromDateTime(DateTime.Now)))
                .ToDictionaryAsync(q => q.ExpenseHeadID, q => q.Amount);

            ViewData["QuotationAmounts"] = quotationAmounts;

            return View(requisition);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requisition = await _context.Requisition.FindAsync(id);
            if (requisition == null)
            {
                return NotFound();
            }
            return View(requisition);
        }

        // POST: Requisitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var requisition = await _context.Requisition.FindAsync(id);
            if (requisition != null)
            {
                _context.Requisition.Remove(requisition);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequisitionExists(int id)
        {
            return _context.Requisition.Any(e => e.Id == id);
        }

        /// ////////////////////////////////////////////////////////////////////////////////////
 
        public IActionResult Upsert(int? id)
        {
            ViewBag.EmployeeList = new SelectList(_context.Employee, "Id", "Name");
            ViewBag.ExpenseHeadList = new SelectList(_context.ExpenseHead, "Id", "ExpenseName");

            // For autocomplete or dropdown
            ViewBag.beList = _context.BEdetails
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.BENo
                })
                .ToList();

            if (id == null || id == 0)
            {
                // Create
                var newRequisition = new Requisition
                {
                    RequisitionDate = DateOnly.FromDateTime(DateTime.Now),
                    RequisitionDetails = new List<RequisitionDetails>()
                };
                return View("Upsert", newRequisition);
            }
            else
            {
                // Edit
                var requisition = _context.Requisition
                    .Include(r => r.Bedetails)
                    .Include(r => r.RequisitionDetails)
                    .ThenInclude(rd => rd.ExpenseHead)
                    .FirstOrDefault(r => r.Id == id);

                if (requisition == null)
                {
                    return NotFound();
                }

                return View("Upsert", requisition);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Requisition requisition)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (ModelState.IsValid)
                {
                    if (requisition.Id == 0)
                    {
                        // Create new Requisition
                        _context.Requisition.Add(requisition);
                        _context.SaveChanges(); // Save early to get generated Id
                    }
                    else
                    {
                        // Update existing Requisition
                        var existing = _context.Requisition
                            .Include(r => r.Bedetails)
                            .Include(r => r.RequisitionDetails)
                            .FirstOrDefault(r => r.Id == requisition.Id);

                        if (existing == null)
                            return NotFound();

                        // Update main fields
                        existing.EmployeeId = requisition.EmployeeId;
                        existing.RequisitionNo = requisition.RequisitionNo;
                        existing.RequisitionDate = requisition.RequisitionDate;
                        existing.BEdetailsID = requisition.BEdetailsID;

                        // Replace RequisitionDetails
                        _context.RequisitionDetails.RemoveRange(existing.RequisitionDetails);
                        existing.RequisitionDetails = requisition.RequisitionDetails;

                        _context.SaveChanges(); // Save updates
                    }

                    // Link selected Expense (if any) to this requisition
                    if (requisition.ExpenseID > 0)
                    {
                        var expense = _context.Expenses.FirstOrDefault(e => e.Id == requisition.ExpenseID);
                        if (expense != null)
                        {
                            expense.RequisitionID = requisition.Id;
                            _context.SaveChanges();
                        }
                    }

                    transaction.Commit();
                    return RedirectToAction("Index");
                }

                // Repopulate dropdowns on ModelState failure
                ViewBag.EmployeeList = new SelectList(_context.Employee, "Id", "Name", requisition.EmployeeId);
                ViewBag.ExpenseHeadList = new SelectList(_context.ExpenseHead, "Id", "ExpenseName");
                ViewBag.beList = _context.BEdetails
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.BENo
                    })
                    .ToList();

                return View("Upsert", requisition);
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                // Log the error (optional)
                ModelState.AddModelError("", "An error occurred while saving the requisition.");
                ViewBag.EmployeeList = new SelectList(_context.Employee, "Id", "Name", requisition.EmployeeId);
                ViewBag.ExpenseHeadList = new SelectList(_context.ExpenseHead, "Id", "ExpenseName");
                ViewBag.beList = _context.BEdetails
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.BENo
                    })
                    .ToList();

                return View("Upsert", requisition);
            }
        }


        [HttpGet]
        public async Task<JsonResult> GetExpensesByBEId(int beId)
        {
            var expenses = await _context.Expenses
                .Include(e => e.Employee)
                .Where(e => e.BEdetailsID == beId)
                .OrderBy(e => e.BillNo)
                .Select(e => new
                {
                    Id = e.Id,
                    EmployeeName = e.Employee.Name,
                    BillType = e.BillType,
                    Amount = e.Amount
                })
                .ToListAsync();

            return Json(expenses);
        }
        [HttpGet]
        public IActionResult DownloadStructuredPdf(int id)
        {
            var requisition = _context.Requisition
                .Include(r => r.Employee)
                .Include(r => r.Bedetails)
                .Include(r => r.RequisitionDetails)
                    .ThenInclude(rd => rd.ExpenseHead)
                .FirstOrDefault(r => r.Id == id);

            if (requisition == null)
                return NotFound();

            // Get active quotation amounts grouped by ExpenseHeadID
            var today = DateOnly.FromDateTime(DateTime.Today);

            var quotationAmounts = _context.Quotation
                .Where(q =>
                    q.EffectiveDate <= today &&
                    (q.ExpiryDate == null || q.ExpiryDate >= today))
                .GroupBy(q => q.ExpenseHeadID)
                .Select(g => new
                {
                    ExpenseHeadId = g.Key,
                    LatestAmount = g.OrderByDescending(q => q.EffectiveDate).FirstOrDefault()!.Amount
                })
                .ToDictionary(q => q.ExpenseHeadId, q => q.LatestAmount);

            var pdfBytes = RequisitionPdfGenerator.CreatePdf(requisition, quotationAmounts);

            Response.Headers["Content-Disposition"] = $"inline; filename=Requisition_{requisition.RequisitionNo}.pdf";
            return File(pdfBytes, "application/pdf");
        }

    }
}
