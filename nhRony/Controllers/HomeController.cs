using ClearingAndForwarding.Context;
using ClearingAndForwarding.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ClearingAndForwarding.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExpensesController> _logger;

        // Constructor to inject the DbContext
        public HomeController(ApplicationDbContext context, ILogger<ExpensesController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            // Check if the user is authenticated
            string? username = User.Identity.IsAuthenticated ? User.Identity.Name : null;
            ViewBag.Username = username;

            return View();
        }
        public IActionResult import()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile fileUpload)
        {
            if (fileUpload == null || fileUpload.Length == 0)
            {
                ViewBag.Message = "Please upload a valid file.";
                return View();
            }

            try
            {
                var expensesList = new List<Expenses>();

                // Preload lookup dictionaries for efficient lookups
                var employees = await _context.Employee
                    .ToDictionaryAsync(e => e.Name.Trim(), e => e.Id, StringComparer.OrdinalIgnoreCase);

                // IMPORTANT: Make sure BEdetails are already imported before this step
                var bedetails = await _context.BEdetails
                    .GroupBy(e => e.BENo)
                    .ToDictionaryAsync(g => g.Key.Trim(), g => g.FirstOrDefault().Id, StringComparer.OrdinalIgnoreCase);

                var existingExpenses = await _context.Expenses
                    .Select(e => new { e.BEno, e.FileNo, e.BillNo })
                    .ToHashSetAsync();

                // Read CSV lines
                using var reader = new StreamReader(fileUpload.OpenReadStream(), Encoding.UTF8);
                var csvData = new List<string[]>();
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        csvData.Add(line.Split(','));
                    }
                }

                if (csvData.Count < 2)
                {
                    ViewBag.Message = "The file is empty or missing headers.";
                    return View();
                }

                var headers = csvData[0];
                var rows = csvData.Skip(1).ToList();

                // Step 1: Build Expenses list (do not assign ExpenseDetails yet)
                foreach (var values in rows)
                {
                    // Parse values safely
                    DateOnly? billDate = ParseDateOnly(values[2]);
                    DateTime? postingDate = ParseDateTime(values[2]);
                    decimal? amount = ParseDecimal(values[65]);

                    var employeeName = values[3].Trim();
                    var employeeId = employees.TryGetValue(employeeName, out var empId) ? empId : 0;

                    var beNoStr = values[4].Trim();
                    var beNo = 0;
                    int.TryParse(beNoStr, out beNo);

                    // Lookup BEdetails ID by BENo string
                    var BEID = bedetails.TryGetValue(beNoStr, out var bid) ? bid : 0;


                    // Skip if expense already exists (based on key properties)
                    if (existingExpenses.Any(e => e.BEno == beNo && e.FileNo == values[1] && e.BillNo == values[7]))
                    {
                        continue;
                    }

                    var expense = new Expenses
                    {
                        BEno = beNo,
                        FileNo = values[1],
                        BillType = values[6],
                        BillDate = billDate ?? new DateOnly(2021, 1, 1),
                        BillNo = values[7],
                        PostingDate = postingDate ?? DateTime.Now,

                        EmployeeId = employeeId,
                        Amount = amount ?? 0m,
                        BEdetailsID = BEID
                    };

                    expensesList.Add(expense);
                }

                if (!expensesList.Any())
                {
                    ViewBag.Message = "No new expenses to import.";
                    return View();
                }

                // Step 2: Save Expenses to DB to generate IDs
                await _context.Expenses.AddRangeAsync(expensesList);
                await _context.SaveChangesAsync();

                // Step 3: Build ExpenseDetails list now that Expense IDs exist
                var expenseDetailList = new List<ExpenseDetails>();

                for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                {
                    var values = rows[rowIndex];

                    // Check if this row corresponds to a saved expense
                    if (rowIndex >= expensesList.Count)
                    {
                        // Shouldn't happen, but skip if index out of bounds
                        continue;
                    }

                    var expense = expensesList[rowIndex];

                    for (int i = 5; i < 65 && i < values.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(values[i]) && decimal.TryParse(values[i], out var detailAmount))
                        {
                            // Header must be numeric (ExpenseHeadId)
                            if (headers[i].All(char.IsDigit) && int.TryParse(headers[i], out var expenseHeadId))
                            {
                                var expenseDetail = new ExpenseDetails
                                {
                                    ExpenseId = expense.Id, // Use DB-generated Expense Id
                                    ExpenseHeadId = expenseHeadId,
                                    Amount = detailAmount
                                };

                                expenseDetailList.Add(expenseDetail);
                            }
                            else
                            {
                                // You can log or ignore invalid headers here
                            }
                        }
                    }
                }

                // Step 4: Save ExpenseDetails
                if (expenseDetailList.Any())
                {
                    await _context.ExpenseDetails.AddRangeAsync(expenseDetailList);
                    await _context.SaveChangesAsync();
                }

                ViewBag.Message = $"File uploaded successfully! Imported {expensesList.Count} expenses and {expenseDetailList.Count} expense details.";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during file import");
                ViewBag.Message = "An error occurred while processing the file. Please try again.";
                return View();
            }
        }

        private DateOnly? ParseDateOnly(string value)
        {
            return DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)
                ? result
                : (DateOnly?)null;
        }

        private DateTime? ParseDateTime(string value)
        {
            return DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)
                ? result
                : (DateTime?)null;
        }

        private decimal? ParseDecimal(string value)
        {
            return decimal.TryParse(value, out var result) ? result : (decimal?)null;
        }
    }
}