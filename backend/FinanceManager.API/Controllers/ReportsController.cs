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
    [Authorize]  // ✅ JWT protected
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ 1. Get monthly income vs expense
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyReport()
        {
            var userId = _userManager.GetUserId(User);

            var monthlyReport = await _context.Transactions
                .Where(t => t.UserId == userId)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalIncome = g.Where(t => t.Type == "Income").Sum(t => t.Amount),
                    TotalExpense = g.Where(t => t.Type == "Expense").Sum(t => t.Amount),
                    Balance = g.Where(t => t.Type == "Income").Sum(t => t.Amount) -
                              g.Where(t => t.Type == "Expense").Sum(t => t.Amount)
                })
                .OrderBy(r => r.Year).ThenBy(r => r.Month)
                .ToListAsync();

            return Ok(monthlyReport);
        }

        // ✅ 2. Get category-wise expense breakdown
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategoryReport()
        {
            var userId = _userManager.GetUserId(User);

            var categoryReport = await _context.Transactions
                .Where(t => t.UserId == userId && t.Type == "Expense")
                .GroupBy(t => t.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalExpense = g.Sum(t => t.Amount)
                })
                .OrderByDescending(r => r.TotalExpense)
                .ToListAsync();

            return Ok(categoryReport);
        }

        // ✅ 3. Get overall balance trend
        [HttpGet("balance-trend")]
        public async Task<IActionResult> GetBalanceTrend()
        {
            var userId = _userManager.GetUserId(User);

            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.Date)
                .ToListAsync();

            decimal runningBalance = 0;
            var trend = transactions.Select(t =>
            {
                runningBalance += (t.Type == "Income" ? t.Amount : -t.Amount);
                return new
                {
                    t.Date,
                    Balance = runningBalance
                };
            });

            return Ok(trend);
        }
    }
}
