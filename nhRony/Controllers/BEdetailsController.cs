using ClearingAndForwarding.Context;
using ClearingAndForwarding.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class BEdetailsController : Controller
{
    private readonly ApplicationDbContext _context;

    public BEdetailsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Import()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Import(IFormFile fileUpload)
    {
        if (fileUpload == null || fileUpload.Length == 0)
        {
            ViewBag.Message = "Please upload a valid CSV file.";
            return View();
        }

        var shipmentDetails = new List<BEdetails>();
        var currentUserName = User.Identity?.Name ?? "UnknownUser";

        try
        {
            using var reader = new StreamReader(fileUpload.OpenReadStream(), Encoding.UTF8);
            var importedKeys = new HashSet<(string BENo, DateOnly BEDate)>();
            var allRows = new List<string[]>();
            bool isHeader = true;

            // Read and store all rows first
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = line.Split(',');

                if (isHeader)
                {
                    isHeader = false;
                    continue;
                }

                if (values.Length < 21) continue;

                var beno = values[1];
                var bedate = ParseDateOnly(values[2]);

                if (!string.IsNullOrWhiteSpace(beno))
                {
                    importedKeys.Add((beno, bedate));
                    allRows.Add(values);
                }
            }

            // Fetch existing BENo + BEDate combinations from the database
            var existingKeys = new HashSet<(string BENo, DateOnly BEDate)>(
            (await _context.BEdetails
                .Where(be => importedKeys.Select(k => k.BENo).Contains(be.BENo))
                .Select(be => new { be.BENo, be.BEDate })
                .ToListAsync()) // <- async DB call
                .Select(be => (be.BENo, be.BEDate)) // now safe to use LINQ-to-Objects
            );


            int skipped = 0, added = 0;

            foreach (var values in allRows)
            {
                var beno = values[1];
                var bedate = ParseDateOnly(values[2]);

                if (existingKeys.Contains((beno, bedate)))
                {
                    skipped++;
                    continue; // Skip existing
                }

                shipmentDetails.Add(new BEdetails
                {
                    FileNo = values[0],
                    BENo = beno,
                    BEDate = bedate,
                    LCNo = values[3],
                    LCDate = ParseDateOnly(values[4]),
                    ItemName = values[5],
                    Quantity = values[6],
                    UOM = values[7],
                    Currency = values[8],
                    Amount = ParseDecimal(values[9]),
                    InvoiceNo = values[10],
                    InvoiceDate = ParseDateOnly(values[11]),
                    UnitCode = values[12],
                    AssessableValue = ParseDecimal(values[13]),
                    Weight = ParseDecimal(values[14]),
                    Shipper = values[15],
                    ArrivalDate = ParseDateOnly(values[16]),
                    ODRecDate = ParseDateOnly(values[17]),
                    AssessmentDate = ParseDateOnly(values[18]),
                    DODate = ParseDateOnly(values[19]),
                    DeliveryDate = ParseDateOnly(values[20]),
                    UserCode = currentUserName
                });

                added++;
            }

            if (shipmentDetails.Any())
            {
                await _context.BEdetails.AddRangeAsync(shipmentDetails);
                await _context.SaveChangesAsync();
            }

            ViewBag.Message = $"Import completed. Added: {added}, Skipped (existing): {skipped}";
        }
        catch (Exception ex)
        {
            ViewBag.Message = $"Error occurred: {ex.Message}";
            if (ex.InnerException != null)
            {
                ViewBag.Message += $" Inner Exception: {ex.InnerException.Message}";
            }
        }

        return View();
    }


    private DateOnly ParseDateOnly(string value)
    {
        return DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)
            ? result
            : new DateOnly(2021, 1, 1); // Default fallback
    }

    private decimal? ParseDecimal(string value)
    {
        return decimal.TryParse(value, out var result) ? result : null;
    }


    public IActionResult Index(string searchString, int pageNumber = 1)
    {
        int pageSize = 13;
        var query = _context.BEdetails.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(b => b.FileNo.Contains(searchString));
        }

        var totalItems = query.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var data = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = new BEdetailsViewModel
        {
            BEdetails = data,
            PageNumber = pageNumber,
            TotalPages = totalPages,
            SearchString = searchString
        };

        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Upsert(int? id, string? returnUrl = null, int? BEID = null)
    {
        // If returnUrl is not passed, get it from Referer header
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Request.Headers["Referer"].ToString();

        // Check if referrer was BEdetails Index page
        bool cameFromIndex = !string.IsNullOrEmpty(returnUrl) &&
                             returnUrl.Contains("/BEdetails/Index", StringComparison.OrdinalIgnoreCase);

        // Store in ViewData
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["CameFromIndex"] = cameFromIndex;
        var deliveryTypes = await _context.DeliveryTypes.ToListAsync();
        ViewBag.DeliveryTypes = new SelectList(deliveryTypes, "Id", "DeliveryTypeName");
        var ItemCategory = await _context.ItemCategories.ToListAsync();
        ViewBag.ItemCategories = new SelectList(ItemCategory, "Id", "CategoryName");

            if (id == null)
        {
            var model = new BEdetails();

            if (BEID.HasValue)
            {
                model.Id = BEID.Value;
                ViewData["BEdetailsID"] = BEID.Value;
            }

            return View(model);
        }

        var bedetail = await _context.BEdetails.FindAsync(id);
        if (bedetail == null)
            return NotFound();

        ViewData["BEdetailsID"] = bedetail.Id;
        return View(bedetail);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(BEdetails bedetail, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["BEdetailsID"] = bedetail.Id;
            return View(bedetail);
        }

        bool isNew = bedetail.Id == 0;

        if (isNew)
            _context.Add(bedetail);
        else
            _context.Update(bedetail);

        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(returnUrl))
        {
            // If coming from BEdetails Index, redirect to Index
            if (returnUrl.Contains("/BEdetails", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction(nameof(Index));

            // Else go back to original referrer (like Expenses/Details)
            return Redirect(returnUrl);
        }

        // Fallbacks
        return isNew
            ? RedirectToAction(nameof(Index))
            : RedirectToAction("Details", "Expenses", new { id = bedetail.Id });
    }

}
