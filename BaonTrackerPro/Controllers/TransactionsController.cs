using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaonTrackerPro.Data;
using BaonTrackerPro.Models;
using System.Security.Claims;

namespace BaonTrackerPro.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index(string searchString, string type)
        {
            var userId = GetCurrentUserId();
            IQueryable<Transaction> transactions = _context.Transactions
                .Where(t => t.AppUserId == userId)
                .OrderByDescending(t => t.Date);

            if (!string.IsNullOrEmpty(searchString))
            {
                transactions = transactions.Where(t =>
                    (t.Description != null && t.Description.Contains(searchString)) ||
                    (t.Category != null && t.Category.Contains(searchString)));
            }

            if (!string.IsNullOrEmpty(type))
            {
                if (type == "Income")
                    transactions = transactions.Where(t => t.Amount >= 0);
                else if (type == "Expense")
                    transactions = transactions.Where(t => t.Amount < 0);
            }

            var result = await transactions.ToListAsync();
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentType"] = type;
            return View(result);
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var userId = GetCurrentUserId();
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id && m.AppUserId == userId);
            if (transaction == null) return NotFound();
            return View(transaction);
        }
        // GET: Transactions/DetailsJson/5
        [HttpGet]
        public async Task<IActionResult> DetailsJson(int id)
        {
            var userId = GetCurrentUserId();
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.AppUserId == userId);
            if (transaction == null) return NotFound();

            return Json(new
            {
                description = transaction.Description,
                category = transaction.Category,
                amount = transaction.Amount,
                absAmount = Math.Abs(transaction.Amount).ToString("N2"),
                isExpense = transaction.Amount < 0,
                date = transaction.Date.ToString("MM/dd/yyyy"),
                time = transaction.Date.ToString("h:mm tt"),
                notes = transaction.Notes
            });
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Transactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,Category,Date,Description,Notes")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.AppUserId = GetCurrentUserId();
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Please fill in all required fields correctly.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var userId = GetCurrentUserId();
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.AppUserId == userId);
            if (transaction == null) return NotFound();
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,Category,Date,Description,Notes")] Transaction transaction)
        {
            if (id != transaction.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = GetCurrentUserId();
                    var existing = await _context.Transactions.AsNoTracking()
                        .FirstOrDefaultAsync(t => t.Id == transaction.Id && t.AppUserId == userId);
                    if (existing == null) return NotFound();

                    // Preserve ownership + fields not included in Bind.
                    transaction.AppUserId = existing.AppUserId;
                    transaction.IsIncome = existing.IsIncome;
                    transaction.CategoryIcon = existing.CategoryIcon;

                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var userId = GetCurrentUserId();
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id && m.AppUserId == userId);
            if (transaction == null) return NotFound();
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.AppUserId == userId);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }

        // POST: Transactions/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([Bind("Id,Amount,Category,Date,Description,Notes")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.Id == 0)
                {
                    transaction.AppUserId = GetCurrentUserId();
                    _context.Add(transaction);
                }
                else
                {
                    var userId = GetCurrentUserId();
                    var existing = await _context.Transactions.AsNoTracking()
                        .FirstOrDefaultAsync(t => t.Id == transaction.Id && t.AppUserId == userId);
                    if (existing == null) return NotFound();

                    transaction.AppUserId = existing.AppUserId ?? userId;
                    _context.Update(transaction);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Please fill in all required fields correctly.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Transactions/GetTransaction/5
        [HttpGet]
        public async Task<IActionResult> GetTransaction(int id)
        {
            var userId = GetCurrentUserId();
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.AppUserId == userId);
            if (transaction == null) return NotFound();

            return Json(new
            {
                id = transaction.Id,
                description = transaction.Description,
                amount = Math.Abs(transaction.Amount),
                category = transaction.Category,
                date = transaction.Date.ToString("yyyy-MM-dd"),
                time = transaction.Date.ToString("HH:mm"),
                notes = transaction.Notes,
                isExpense = transaction.Amount < 0
            });
        }

        // GET: Transactions/History
        public async Task<IActionResult> History(string? type, string? sort, string? month)
        {
            var selectedMonth = string.IsNullOrEmpty(month)
                ? DateTime.Now.ToString("yyyy-MM")
                : month;

            var parsedMonth = DateTime.ParseExact(selectedMonth, "yyyy-MM", null);

            var userId = GetCurrentUserId();
            IQueryable<Transaction> transactions = _context.Transactions
                .Where(t => t.AppUserId == userId &&
                            t.Date.Year == parsedMonth.Year && t.Date.Month == parsedMonth.Month);

            if (type == "Income")
                transactions = transactions.Where(t => t.Amount >= 0);
            else if (type == "Expense")
                transactions = transactions.Where(t => t.Amount < 0);

            transactions = sort switch
            {
                "amount_asc" => transactions.OrderBy(t => t.Amount),
                "amount_desc" => transactions.OrderByDescending(t => t.Amount),
                "date_asc" => transactions.OrderBy(t => t.Date),
                _ => transactions.OrderByDescending(t => t.Date)
            };

            var result = await transactions.ToListAsync();

            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.SelectedType = type ?? "";
            ViewBag.SelectedSort = sort ?? "date_desc";
            ViewBag.TotalSpent = result.Where(t => t.Amount < 0).Sum(t => t.Amount);
            ViewBag.TotalReceived = result.Where(t => t.Amount >= 0).Sum(t => t.Amount);
            ViewBag.Net = result.Sum(t => t.Amount);

            return View(result);
        }

        private int GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : 0;
        }

    } // 👈 only ONE closing brace for the class
}