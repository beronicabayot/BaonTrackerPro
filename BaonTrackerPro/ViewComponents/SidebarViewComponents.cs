using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaonTrackerPro.Data;   // <-- make sure this matches your project's namespace

namespace BaonTrackerPro.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public SidebarViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var totalExpenses = await _context.Transactions
                .Where(t => t.Date >= startOfMonth && t.Date <= endOfMonth)
                .SumAsync(t => t.Amount);

            var totalBudget = await _context.BudgetItems.SumAsync(b => b.MonthlyAmount);

            var remainingBudget = totalBudget - totalExpenses;

            return View(remainingBudget);
        }
    }
}