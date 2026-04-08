using BaonTrackerPro.Data;
using BaonTrackerPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            var userId = GetCurrentUserId();
            var goals = await _context.SavingsGoals
                .Where(g => g.AppUserId == userId && !g.IsDone)
                .ToListAsync();

            var transactions = await _context.Transactions
                .Where(t => t.AppUserId == userId)
                .ToListAsync();
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
            var userId = GetCurrentUserId();
            var completedGoals = await _context.SavingsGoals
                .Where(g => g.AppUserId == userId && g.IsDone)
                .ToListAsync();
            return View(completedGoals);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsDone(int id)
        {
            var userId = GetCurrentUserId();
            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.AppUserId == userId);
            if (goal != null)
            {
                goal.IsDone = true;
                goal.CurrentAmount = goal.TargetAmount;

                // Add as expense transaction so net is deducted
                var transaction = new Transaction
                {
                    AppUserId = userId,
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
                model.AppUserId = GetCurrentUserId();
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
                var userId = GetCurrentUserId();
                var existing = await _context.SavingsGoals.AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Id == model.Id && g.AppUserId == userId);
                if (existing == null) return NotFound();

                model.AppUserId = existing.AppUserId ?? userId;
                _context.SavingsGoals.Update(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddFunds(int id, decimal amount, string fundAction)
        {
            var userId = GetCurrentUserId();
            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.AppUserId == userId);
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
            var userId = GetCurrentUserId();
            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.AppUserId == userId);
            if (goal != null)
            {
                _context.SavingsGoals.Remove(goal);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        private int GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : 0;
        }
    }
}