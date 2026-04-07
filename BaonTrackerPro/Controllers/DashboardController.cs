using Microsoft.AspNetCore.Mvc;
using BaonTrackerPro.Models;

namespace BaonTrackerPro.Controllers
{
public class DashboardController : Controller    {
        public IActionResult Index()
        {
            var sampleTransactions = new List<Transaction>
            {
                new() { Id=1, Description="Mga Damit", Category="Personal/Lifestyle",
                        CategoryIcon="🛍", Amount=2000.00m, IsIncome=false,
                        Date=DateTime.Now.AddDays(-1) },
                new() { Id=2, Description="Sumakay papuntang school", Category="Transportation",
                        CategoryIcon="🚌", Amount=20.00m, IsIncome=false,
                        Date=DateTime.Now.AddDays(-1) },
                new() { Id=3, Description="Allowance", Category="Allowance",
                        CategoryIcon="💵", Amount=5000.00m, IsIncome=true,
                        Date=DateTime.Now.AddDays(-1) },
            };

            var income   = sampleTransactions.Where(t => t.IsIncome).Sum(t => t.Amount);
            var expenses = sampleTransactions.Where(t => !t.IsIncome).Sum(t => t.Amount);

            var vm = new FinanceDashboardViewModel
            {
                UserName     = "Beronica",
                Income       = income,
                Expenses     = expenses,
                Balance      = income - expenses,
                SavingsGoals = 0,
                RecentTransactions = sampleTransactions,
                SpendingByCategory = new List<SpendingCategory>
                {
                    new() { Name="Personal/Lifestyle", Percentage=90, Color="#e91e8c", Icon="🛍" },
                    new() { Name="Transportation",     Percentage=10, Color="#f59e0b", Icon="🚌" },
                },
                Goals = new List<SavingsGoal>()
            };

            return View(vm);  // ← passing the model here
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTransaction(Transaction model)
        {
            TempData["Success"] = "Transaction added!";
            return RedirectToAction("Index");
        }
    }
}