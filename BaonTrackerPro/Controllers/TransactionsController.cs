using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaonTrackerPro.Data;
using BaonTrackerPro.Models;

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
            IQueryable<Transaction> transactions = _context.Transactions.OrderByDescending(t => t.Date);

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
            var transaction = await _context.Transactions.FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null) return NotFound();
            return View(transaction);
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
            var transaction = await _context.Transactions.FindAsync(id);
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
            var transaction = await _context.Transactions.FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null) return NotFound();
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
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
                    _context.Add(transaction);
                else
                    _context.Update(transaction);

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
            var transaction = await _context.Transactions.FindAsync(id);
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

            IQueryable<Transaction> transactions = _context.Transactions
                .Where(t => t.Date.Year == parsedMonth.Year && t.Date.Month == parsedMonth.Month);

            if (type == "Income")
                transactions = transactions.Where(t => t.Amount >= 0);
            else if (type == "Expense")
                transactions = transactions.Where(t => t.Amount < 0);

            transactions = sort switch
            {
                "amount_asc"  => transactions.OrderBy(t => t.Amount),
                "amount_desc" => transactions.OrderByDescending(t => t.Amount),
                "date_asc"    => transactions.OrderBy(t => t.Date),
                _             => transactions.OrderByDescending(t => t.Date)
            };

            var result = await transactions.ToListAsync();

            ViewBag.SelectedMonth    = selectedMonth;
            ViewBag.SelectedType     = type ?? "";
            ViewBag.SelectedSort     = sort ?? "date_desc";
            ViewBag.TotalSpent       = result.Where(t => t.Amount < 0).Sum(t => t.Amount);
            ViewBag.TotalReceived    = result.Where(t => t.Amount >= 0).Sum(t => t.Amount);
            ViewBag.Net              = result.Sum(t => t.Amount);

            return View(result);
        }

    } // 👈 only ONE closing brace for the class
}