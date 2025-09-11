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
    public class BadgesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BadgesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ Get all badges for the logged-in user
        [HttpGet]
        public async Task<IActionResult> GetBadges()
        {
            var userId = _userManager.GetUserId(User);
            var badges = await _context.Badges.Where(b => b.UserId == userId).ToListAsync();
            return Ok(badges);
        }
    }
}
