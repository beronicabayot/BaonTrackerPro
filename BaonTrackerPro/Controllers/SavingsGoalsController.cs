using BaonTrackerPro.Data;
using BaonTrackerPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BaonTrackerPro.Controllers
{
    // Must be exactly this name
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
            return View(goals);
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
    }
}