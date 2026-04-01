using BaonTrackerPro.Data;
using BaonTrackerPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaonTrackerPro.Controllers
{
    public class SavingsGoalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SavingsGoalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var goals = await _context.SavingsGoals
                .Where(g => !g.IsDone)
                .ToListAsync();

            var transactions = await _context.Transactions.ToListAsync();
            var totalIncome = transactions.Where(t => t.Amount >= 0).Sum(t => t.Amount);
            var totalExpense = transactions.Where(t => t.Amount < 0).Sum(t => t.Amount);
            var net = totalIncome + totalExpense;

            var expectedSpent = goals.Sum(g => g.TargetAmount);
            var expectedNet = net - expectedSpent;

            ViewBag.Net = net;
            ViewBag.ExpectedSpent = expectedSpent;
            ViewBag.ExpectedNet = expectedNet;

            return View(goals);
        }

        public async Task<IActionResult> Completed()
        {
            var completedGoals = await _context.SavingsGoals
                .Where(g => g.IsDone)
                .ToListAsync();
            return View(completedGoals);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsDone(int id)
        {
            var goal = await _context.SavingsGoals.FindAsync(id);
            if (goal != null)
            {
                goal.IsDone = true;
                goal.CurrentAmount = goal.TargetAmount;

                // Add as expense transaction so net is deducted
                var transaction = new Transaction
                {
                    Description = $"Savings Goal: {goal.Name}",
                    Amount = -goal.TargetAmount,
                    Category = "Savings Goal",
                    Date = DateTime.Now,
                    Notes = "Marked as done from Savings Goals"
                };
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Create(SavingsGoal model)
        {
            if (ModelState.IsValid)
            {
                _context.SavingsGoals.Add(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SavingsGoal model)
        {
            if (ModelState.IsValid)
            {
                _context.SavingsGoals.Update(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddFunds(int id, decimal amount, string fundAction)
        {
            var goal = await _context.SavingsGoals.FindAsync(id);
            if (goal != null)
            {
                if (fundAction == "deduct")
                {
                    goal.CurrentAmount -= amount;
                    if (goal.CurrentAmount < 0)
                        goal.CurrentAmount = 0;
                }
                else
                {
                    goal.CurrentAmount += amount;
                    if (goal.CurrentAmount > goal.TargetAmount)
                        goal.CurrentAmount = goal.TargetAmount;
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var goal = await _context.SavingsGoals.FindAsync(id);
            if (goal != null)
            {
                _context.SavingsGoals.Remove(goal);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}