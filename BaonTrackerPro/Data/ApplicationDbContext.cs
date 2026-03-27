using Microsoft.EntityFrameworkCore;
using BaonTrackerPro.Models;

namespace BaonTrackerPro.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BudgetItem> BudgetItems { get; set; }
        public DbSet<SavingsGoal> SavingsGoals { get; set; }
    }
}