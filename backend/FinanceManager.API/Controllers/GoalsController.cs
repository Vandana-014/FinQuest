using FinanceManager.API.Data;
using FinanceManager.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GoalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GoalsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ Get all goals for logged-in user
        [HttpGet]
        public async Task<IActionResult> GetGoals()
        {
            var userId = _userManager.GetUserId(User);
            var goals = await _context.Goals.Where(g => g.UserId == userId).ToListAsync();
            return Ok(goals);
        }

        // ✅ Create a new goal
        [HttpPost]
        public async Task<IActionResult> CreateGoal(Goal goal)
        {
            var userId = _userManager.GetUserId(User);
            goal.UserId = userId;
            goal.Status = "InProgress";
            goal.StartDate = DateTime.UtcNow;

            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();

            return Ok(goal);
        }

        // ✅ Update progress
        [HttpPut("{id}/progress")]
        public async Task<IActionResult> UpdateGoalProgress(int id, [FromBody] decimal amount)
        {
            var userId = _userManager.GetUserId(User);
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (goal == null) return NotFound();

            goal.CurrentAmount += amount;

            if (goal.CurrentAmount >= goal.TargetAmount)
            {
                goal.Status = "Completed";
                goal.EndDate = DateTime.UtcNow;

                // 🎖️ Award a badge when the goal is completed
                var badge = new Badge
                {
                    UserId = userId,
                    Name = "Goal Crusher",
                    Description = $"Completed goal: {goal.Title}",
                    DateUnlocked = DateTime.UtcNow
                };

                _context.Badges.Add(badge);
            }

            await _context.SaveChangesAsync();
            return Ok(goal);
        }


        // ✅ Mark as failed (if deadline missed)
        [HttpPut("{id}/fail")]
        public async Task<IActionResult> MarkAsFailed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (goal == null) return NotFound();

            goal.Status = "Failed";
            goal.EndDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(goal);
        }

        // ✅ Delete goal
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            var userId = _userManager.GetUserId(User);
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (goal == null) return NotFound();

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Goal deleted successfully" });
        }
    }
}
