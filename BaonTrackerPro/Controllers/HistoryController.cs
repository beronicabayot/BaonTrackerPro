using BaonTrackerPro.Data;
using BaonTrackerPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            IQueryable<Transaction> transactions = _context.Transactions
                .Where(t => t.Date.Year == parsedMonth.Year && t.Date.Month == parsedMonth.Month);

            if (type == "Income")
                transactions = transactions.Where(t => t.Amount >= 0);
            else if (type == "Expense")
                transactions = transactions.Where(t => t.Amount < 0);

            // Fetch first, then sort in memory to avoid EF translation issues
            var result = await transactions.ToListAsync();

            result = sort switch
            {
                "amount_desc" => result.OrderByDescending(t => Math.Abs(t.Amount)).ToList(),
                "amount_asc"  => result.OrderBy(t => Math.Abs(t.Amount)).ToList(),
                _             => result.OrderByDescending(t => t.Date).ToList()
            };

            ViewBag.SelectedMonth  = selectedMonth;
            ViewBag.SelectedType   = type ?? "";
            ViewBag.SelectedSort   = sort ?? "";
            ViewBag.TotalSpent     = result.Where(t => t.Amount < 0).Sum(t => t.Amount);
            ViewBag.TotalReceived  = result.Where(t => t.Amount >= 0).Sum(t => t.Amount);
            ViewBag.Net            = result.Sum(t => t.Amount);

            return View(result);
        }
    }
}