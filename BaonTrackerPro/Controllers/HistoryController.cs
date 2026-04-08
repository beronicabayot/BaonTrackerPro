using BaonTrackerPro.Data;
using BaonTrackerPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BaonTrackerPro.Controllers
{
    public class HistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? type, string? sort, string? month)
        {
            var selectedMonth = string.IsNullOrEmpty(month)
                ? DateTime.Now.ToString("yyyy-MM")
                : month;

            var parsedMonth = DateTime.ParseExact(selectedMonth, "yyyy-MM", null);
            var userId = GetCurrentUserId();

            // ✅ Fetch ALL transactions for the month first (for summary cards)
            var allForMonth = await _context.Transactions
                .Where(t => t.AppUserId == userId &&
                            t.Date.Year == parsedMonth.Year && t.Date.Month == parsedMonth.Month)
                .ToListAsync();

            // ✅ Then apply filter + sort on a separate query for the list
            IQueryable<Transaction> transactions = _context.Transactions
                .Where(t => t.AppUserId == userId &&
                            t.Date.Year == parsedMonth.Year && t.Date.Month == parsedMonth.Month);

            if (type == "Income")
                transactions = transactions.Where(t => t.Amount >= 0);
            else if (type == "Expense")
                transactions = transactions.Where(t => t.Amount < 0);

            var result = await transactions.ToListAsync();

            result = sort switch
            {
                "amount_desc" => result.OrderByDescending(t => Math.Abs(t.Amount)).ToList(),
                "amount_asc" => result.OrderBy(t => Math.Abs(t.Amount)).ToList(),
                _ => result.OrderByDescending(t => t.Date).ToList()
            };

            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.SelectedType = type ?? "";
            ViewBag.SelectedSort = sort ?? "";

            // ✅ Always based on full month, never filtered
            ViewBag.TotalSpent = allForMonth.Where(t => t.Amount < 0).Sum(t => t.Amount);
            ViewBag.TotalReceived = allForMonth.Where(t => t.Amount >= 0).Sum(t => t.Amount);
            ViewBag.Net = allForMonth.Sum(t => t.Amount);

            return View(result);
        }

        private int GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : 0;
        }
    }
}