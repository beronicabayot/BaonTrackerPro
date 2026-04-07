using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaonTrackerPro.Data;
using BaonTrackerPro.Models;

namespace BaonTrackerPro.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await BuildDashboardViewModel();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var model = await BuildDashboardViewModel();

            return Json(new
            {
                balance = model.Balance,
                income = model.Income,
                expenses = model.Expenses,
                savingsGoals = model.SavingsGoals,
                spendingByCategory = model.SpendingByCategory.Select(c => new
                {
                    name = c.Name,
                    percentage = c.Percentage,
                    color = c.Color,
                    icon = c.Icon
                }),
                goals = model.Goals.Select(g => new
                {
                    name = g.Name,
                    currentAmount = g.CurrentAmount,
                    targetAmount = g.TargetAmount
                }),
                recentTransactions = model.RecentTransactions.Select(t => new
                {
                    description = t.Description,
                    category = t.Category,
                    categoryIcon = t.CategoryIcon,
                    amount = t.Amount,
                    isIncome = t.IsIncome,
                    date = t.Date.ToString("MMM dd")
                })
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTransaction(
            string Description,
            decimal Amount,
            string Category,
            DateTime Date,
            bool IsIncome,
            string? Notes)
        {
            var transaction = new Transaction
            {
                Description = Description,
                Amount = Math.Abs(Amount),
                Category = Category,
                Date = Date,
                Notes = Notes,
                IsIncome = IsIncome,
                CategoryIcon = GetCategoryIcon(Category)
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var dashboard = await BuildDashboardViewModel();

            return Json(new
            {
                success = true,
                balance = dashboard.Balance,
                income = dashboard.Income,
                expenses = dashboard.Expenses,
                savingsGoals = dashboard.SavingsGoals,
                transaction = new
                {
                    description = transaction.Description,
                    amount = transaction.Amount,
                    isIncome = transaction.IsIncome,
                    category = transaction.Category,
                    categoryIcon = transaction.CategoryIcon,
                    date = transaction.Date.ToString("MMM dd")
                }
            });
        }

        private async Task<FinanceDashboardViewModel> BuildDashboardViewModel()
        {
            var transactions = await _context.Transactions
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.Id)
                .ToListAsync();

            var goals = await _context.SavingsGoals
                .OrderBy(g => g.Id)
                .ToListAsync();

            var income = transactions
                .Where(t => t.IsIncome)
                .Sum(t => t.Amount);

            var expenses = transactions
                .Where(t => !t.IsIncome)
                .Sum(t => t.Amount);

            var balance = income - expenses;
            var totalExpense = expenses;

            var spendingByCategory = transactions
                .Where(t => !t.IsIncome)
                .GroupBy(t => t.Category)
                .Select(g => new SpendingCategory
                {
                    Name = g.Key,
                    Percentage = totalExpense <= 0 ? 0 : (g.Sum(x => x.Amount) / totalExpense) * 100,
                    Color = GetCategoryColor(g.Key),
                    Icon = GetCategoryIcon(g.Key)
                })
                .OrderByDescending(x => x.Percentage)
                .ToList();

            foreach (var txn in transactions)
            {
                txn.CategoryIcon = GetCategoryIcon(txn.Category);
            }

            return new FinanceDashboardViewModel
            {
                UserName = "Beronica",
                Balance = balance,
                Income = income,
                Expenses = expenses,
                SavingsGoals = goals.Sum(g => g.CurrentAmount),
                RecentTransactions = transactions.Take(5).ToList(),
                SpendingByCategory = spendingByCategory,
                Goals = goals
            };
        }

        private string GetCategoryIcon(string? category)
        {
            return category switch
            {
                "Food" => "🍔",
                "Transportation" => "🚌",
                "Bills" => "🧾",
                "Health" => "💊",
                "Entertainment" => "🎮",
                "Allowance" => "💵",
                "Personal/Lifestyle" => "🛍",
                _ => "📦"
            };
        }

        private string GetCategoryColor(string? category)
        {
            return category switch
            {
                "Personal/Lifestyle" => "#e91e8c",
                "Transportation" => "#f59e0b",
                "Food" => "#10b981",
                "Allowance" => "#3b82f6",
                "Bills" => "#8b5cf6",
                "Health" => "#ef4444",
                "Entertainment" => "#14b8a6",
                _ => "#6b7280"
            };
        }
    }
}