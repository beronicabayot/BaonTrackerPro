using BaonTrackerPro.Models;

namespace BaonTrackerPro.Models
{
    public class FinanceDashboardViewModel
    {
        public string UserName { get; set; } = "Beronica";
        public decimal Balance { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public decimal SavingsGoals { get; set; }
        public List<Transaction> RecentTransactions { get; set; } = new();
        public List<SpendingCategory> SpendingByCategory { get; set; } = new();
        public List<SavingsGoal> Goals { get; set; } = new();
    }

    public class SpendingCategory
    {
        public string Name { get; set; } = "";
        public decimal Percentage { get; set; }
        public string Color { get; set; } = "#e91e8c";
        public string Icon { get; set; } = "";
    }
}