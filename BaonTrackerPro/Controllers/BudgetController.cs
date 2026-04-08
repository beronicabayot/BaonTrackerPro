using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaonTrackerPro.Data;
using BaonTrackerPro.Models;
using System.Security.Claims;

namespace BaonTrackerPro.Controllers
{
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? month)
        {
            var userId = GetCurrentUserId();
            var selectedMonth = string.IsNullOrEmpty(month)
                ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                : DateTime.ParseExact(month, "yyyy-MM", null);

            var budgets = await _context.BudgetItems
                .Where(b => b.AppUserId == userId &&
                            b.BudgetMonth.Year == selectedMonth.Year &&
                            b.BudgetMonth.Month == selectedMonth.Month)
                .ToListAsync();

            var transactions = await _context.Transactions
                .Where(t => t.AppUserId == userId)
                .ToListAsync();

            var budgetViewModels = budgets.Select(b =>
            {
                var spent = transactions
                    .Where(t => t.Category == b.Category && t.Amount < 0)
                    .Where(t => b.Period == "Weekly"
                        ? t.Date >= selectedMonth.AddDays(-7)
                        : t.Date.Month == selectedMonth.Month && t.Date.Year == selectedMonth.Year)
                    .Sum(t => Math.Abs(t.Amount));

                return new BudgetItemViewModel
                {
                    BudgetItem = b,
                    Spent = spent
                };
            }).ToList();

            ViewBag.SelectedMonth = selectedMonth.ToString("yyyy-MM");
            return View(budgetViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Category,AmountLimit,Period,BudgetMonth")] BudgetItem budget)
        {
            if (ModelState.IsValid)
            {
                budget.BudgetMonth = new DateTime(budget.BudgetMonth.Year, budget.BudgetMonth.Month, 1);
                budget.AppUserId = GetCurrentUserId();
                _context.Add(budget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Please fill in all required fields.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var budget = await _context.BudgetItems
                .FirstOrDefaultAsync(b => b.Id == id && b.AppUserId == userId);
            if (budget != null)
            {
                _context.BudgetItems.Remove(budget);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Category,AmountLimit,Period,BudgetMonth")] BudgetItem budget)
        {
            if (id != budget.Id) return NotFound();
            if (ModelState.IsValid)
            {
                budget.BudgetMonth = new DateTime(budget.BudgetMonth.Year, budget.BudgetMonth.Month, 1);
                var userId = GetCurrentUserId();
                var existing = await _context.BudgetItems.AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == budget.Id && b.AppUserId == userId);
                if (existing == null) return NotFound();

                budget.AppUserId = existing.AppUserId ?? userId;
                _context.Update(budget);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private int GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : 0;
        }
    }

    public class BudgetItemViewModel
    {
        public BudgetItem BudgetItem { get; set; } = new();
        public decimal Spent { get; set; }
        public decimal Percentage => BudgetItem.AmountLimit > 0
            ? Math.Min((Spent / BudgetItem.AmountLimit) * 100, 100)
            : 0;
    }
}
