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
            var goals = await _context.SavingsGoals.ToListAsync();
            var activeGoals = goals
                .Where(g => g.TargetAmount == 0 ||
                       Math.Round((double)g.CurrentAmount / (double)g.TargetAmount * 100, 2) < 100)
                .ToList();
            return View(activeGoals);
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
        public async Task<IActionResult> AddFunds(int id, decimal amount)
        {
            var goal = await _context.SavingsGoals.FindAsync(id);
            if (goal != null)
            {
                goal.CurrentAmount += amount;
                if (goal.CurrentAmount > goal.TargetAmount)
                    goal.CurrentAmount = goal.TargetAmount;
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

        public async Task<IActionResult> Completed()
        {
            var allGoals = await _context.SavingsGoals.ToListAsync();
            var completedGoals = allGoals
                .Where(g => g.TargetAmount > 0 &&
                       Math.Round((double)g.CurrentAmount / (double)g.TargetAmount * 100, 2) >= 100)
                .ToList();
            return View(completedGoals);
        }
    }
}