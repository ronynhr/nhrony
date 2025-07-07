using ClearingAndForwarding.Context;
using ClearingAndForwarding.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace ClearingAndForwarding.Controllers
{
    public class ExpensesController(ApplicationDbContext context, ILogger<ExpensesController> logger) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<ExpensesController> _logger = logger;

        private Dictionary<int, decimal> CalculateTotalAmountsForPage(List<int> expenseIds)
        {
            var excludedExpenseHeadIds = new List<int> { 1, 2, 58, 59, 60 };

            return _context.ExpenseDetails
                .Where(ed => expenseIds.Contains(ed.ExpenseId) && !excludedExpenseHeadIds.Contains(ed.ExpenseHeadId))
                .GroupBy(ed => ed.ExpenseId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(ed => ed.Amount)
                );
        }

        //[Authorize]
        public IActionResult Index(string searchString, int pageNumber = 1, int pageSize = 13)
        {
            // Base query for expenses
            var query = _context.Expenses
                .Where(e => e.EmployeeId != 69)
                .AsQueryable();

            // Query for BEdetails to construct the dictionary
            // var bedetails = _context.BEdetails.ToDictionary(b => b.Id, b => b.BENo);

            // Apply search filter if a search string is provided
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(e =>
                    EF.Functions.Like(e.Employee.Name, $"%{searchString}%") ||
                    EF.Functions.Like(e.Bedetails.BENo.ToString(), $"%{searchString}%") ||
                    EF.Functions.Like(e.FileNo, $"%{searchString}%") ||
                    EF.Functions.Like(e.BillType, $"%{searchString}%") ||
                    EF.Functions.Like(e.BillNo, $"%{searchString}%") ||
                    EF.Functions.Like(e.Amount.ToString(), $"%{searchString}%")
                );
            }

            // Calculate total records and total pages
            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Fetch data for the current page
            var expenses = query
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new Expenses
                {
                    Id = e.Id,
                    BEno = e.BEno,
                    FileNo = e.FileNo,
                    BillType = e.BillType,
                    BillDate = e.BillDate,
                    Employee = new Employee { Name = e.Employee.Name },
                    Amount = e.Amount,
                    BEdetailsID = e.BEdetailsID // Ensure this property is included if needed
                })
                .ToList();

            // Prepare the view model
            var model = new ExpensesViewModel
            {
                SearchString = searchString,
                Expenses = expenses,
                PageNumber = pageNumber,
                TotalPages = totalPages
            };

            // Pass BEdetails dictionary to the view via ViewData
            // ViewData["bedetails"] = bedetails;

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            // Use AsNoTracking since this is a read-only operation
            var expenses = await _context.Expenses
                .AsNoTracking()
                .Include(e => e.ExpenseDetails)
                    .ThenInclude(ed => ed.ExpenseHead)
                .Include(e => e.Employee)
                .Include(e => e.Bedetails)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (expenses == null)
                return NotFound();

            // Limit matching expenses to top 20 to avoid loading too many rows
            var matchingExpenses = await _context.Expenses
                .AsNoTracking()
                .Include(e => e.ExpenseDetails)
                    .ThenInclude(d => d.ExpenseHead)
                .Include(e => e.Employee)
                .Where(e => e.BEdetailsID == expenses.BEdetailsID && e.Id != expenses.Id)
                .Take(20)
                .ToListAsync();

            ViewData["MatchingExpenses"] = matchingExpenses;

            var requisitionAmounts = await _context.Requisition
                .Where(r => r.ExpenseID == id)
                .SelectMany(r => r.RequisitionDetails)
                .GroupBy(rd => rd.ExpenseHeadId)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(rd => rd.Amount));

            ViewData["RequisitionAmounts"] = requisitionAmounts;

            var expenseHeadIds = expenses.ExpenseDetails.Select(ed => ed.ExpenseHeadId).ToList();

            var quotationAmounts = await _context.Quotation
                .AsNoTracking()
                .Where(q =>
                    expenseHeadIds.Contains(q.ExpenseHeadID) &&
                    expenses.BillDate >= q.EffectiveDate &&
                    expenses.BillDate <= (q.ExpiryDate ?? DateOnly.FromDateTime(DateTime.Now)))
                .ToListAsync();

            ViewData["QuotationAmounts"] = quotationAmounts;

            return View(expenses);
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expenses = await _context.Expenses.FindAsync(id);
            if (expenses == null)
            {
                return NotFound();
            }

            _context.Expenses.Remove(expenses);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpensesExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }

        private void PopulateDropdowns()
        {
            ViewBag.EmployeeList = _context.Employee
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                })
                .ToList();

            ViewBag.ExpenseHeadList = _context.ExpenseHead
                .Select(eh => new SelectListItem
                {
                    Value = eh.Id.ToString(),
                    Text = eh.ExpenseName
                })
                .ToList();

            ViewBag.beList = _context.BEdetails
                .Select(eh => new SelectListItem
                {
                    Value = eh.Id.ToString(),
                    Text = eh.BENo
                })
                .ToList();

            ViewBag.BillTypes = _context.BillTypes
                .Select(eh => new SelectListItem
                {
                    Value = eh.BillTypeName,
                    Text = eh.BillTypeName
                })
                .ToList();
        }

        public IActionResult Upsert(int? id)
        {
            Expenses? expense = new();

            if (id == null)
            {
                // Create mode
                PopulateDropdowns();
                return View("Upsert", expense);
            }

            // Edit mode
            expense = _context.Expenses
                .Include(e => e.Requisition)
                .Include(e => e.Requisition.RequisitionDetails)
                .Include(e => e.ExpenseDetails)
                    .ThenInclude(ed => ed.ExpenseHead)
                .FirstOrDefault(e => e.Id == id);

            if (expense == null)
            {
                return NotFound();
            }

            PopulateDropdowns();
            return View("Upsert", expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(int? id, Expenses expense)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View("Upsert", expense);
            }

            if (id == null || id == 0)
            {
                _context.Add(expense);
                await _context.SaveChangesAsync();
            }
            else
            {
                var existingExpense = await _context.Expenses
                    .Include(e => e.ExpenseDetails)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (existingExpense == null)
                {
                    return NotFound();
                }

                // Update ExpenseDetails
                var newDetails = expense.ExpenseDetails;
                var existingDetails = existingExpense.ExpenseDetails;

                var toDelete = existingDetails
                    .Where(ed => !newDetails.Any(nd => nd.Id == ed.Id))
                    .ToList();

                _context.ExpenseDetails.RemoveRange(toDelete);
                await _context.SaveChangesAsync();

                foreach (var existingDetail in existingDetails)
                {
                    var matching = newDetails.FirstOrDefault(nd => nd.Id == existingDetail.Id);
                    if (matching != null)
                    {
                        existingDetail.Amount = matching.Amount;
                        existingDetail.ExpenseHeadId = matching.ExpenseHeadId;
                    }
                }

                var toAdd = newDetails
                    .Where(nd => nd.Id == 0 || !existingDetails.Any(ed => ed.Id == nd.Id))
                    .ToList();

                foreach (var detail in toAdd)
                {
                    detail.ExpenseId = expense.Id;
                    _context.ExpenseDetails.Add(detail);
                }

                // Update main expense fields
                existingExpense.FileNo = expense.FileNo;
                existingExpense.BillType = expense.BillType;
                existingExpense.BillDate = expense.BillDate;
                existingExpense.BillNo = expense.BillNo;
                existingExpense.Amount = expense.Amount;
                existingExpense.EmployeeId = expense.EmployeeId;
                existingExpense.BEdetailsID = expense.BEdetailsID;
                existingExpense.PostingDate = expense.PostingDate;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetRequisitionsWithNoExpense(int beId)
        {
            var data = await _context.Requisition
                .Where(r => r.BEdetailsID == beId)
                .Include(r => r.Employee)
                .Select(r => new
                {
                    id = r.Id,
                    employeeName = r.Employee.Name,
                    requisitionNo = r.Id,
                    amount = r.RequisitionAmount
                })
                .ToListAsync();

            return Json(data);
        }
        [HttpGet]
        public IActionResult GetById(int id)
        {
            var requisition = _context.Requisition
                .Include(r => r.RequisitionDetails)
                .FirstOrDefault(r => r.Id == id);

            if (requisition == null)
                return NotFound();

            var requisitionAmount = requisition.RequisitionDetails.Sum(d => d.Amount);

            return Json(new { requisitionAmount });
        }

    }
}
