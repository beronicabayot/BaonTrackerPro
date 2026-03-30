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

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                transactions = transactions.Where(t =>
                    (t.Description != null && t.Description.Contains(searchString)) ||
                    (t.Category != null && t.Category.Contains(searchString)));
            }

            // Apply type filter (Income / Expense)
            if (!string.IsNullOrEmpty(type))
            {
                if (type == "Income")
                {
                    transactions = transactions.Where(t => t.Amount >= 0);
                }
                else if (type == "Expense")
                {
                    transactions = transactions.Where(t => t.Amount < 0);
                }
            }

            var result = await transactions.ToListAsync();

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentType"] = type;   // keep current type for the dropdown
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

            // If validation fails, add an error message and redirect back to Index
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

        // POST: Transactions/Save (Create or Update)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([Bind("Id,Amount,Category,Date,Description,Notes")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.Id == 0)
                {
                    // New transaction
                    _context.Add(transaction);
                }
                else
                {
                    // Update existing
                    _context.Update(transaction);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, show error
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

    }
}