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
    [Authorize]  // ✅ Protect with JWT
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ GET api/dashboard/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            // Get logged-in user ID from JWT
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found in token.");

            // Fetch transactions for this user
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();

            // Fetch active goals
            var goals = await _context.Goals
                .Where(g => g.UserId == userId && g.Status == "InProgress")
                .ToListAsync();

            // Fetch unlocked badges
            var badges = await _context.Badges
                .Where(b => b.UserId == userId)
                .ToListAsync();

            // Calculate summary
            var totalIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            var totalExpense = transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
            var balance = totalIncome - totalExpense;

            var recentTransactions = transactions
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

            return Ok(new
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = balance,
                RecentTransactions = recentTransactions,
                ActiveGoals = goals.Select(g => new
                {
                    g.Id,
                    g.Title,
                    g.TargetAmount,
                    g.CurrentAmount,
                    Progress = g.TargetAmount > 0 ? (g.CurrentAmount / g.TargetAmount) * 100 : 0
                }),
                TotalBadges = badges.Count
            });
        }
    }
}
