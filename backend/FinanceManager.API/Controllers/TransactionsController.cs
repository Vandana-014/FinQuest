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
    [Authorize]  // 🔒 Require JWT
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ Get all transactions for logged-in user
        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            var userId = _userManager.GetUserId(User);
            var transactions = await _context.Transactions
                                             .Where(t => t.UserId == userId)
                                             .ToListAsync();
            return Ok(transactions);
        }

        // ✅ Add a new transaction
        [HttpPost]
        public async Task<IActionResult> AddTransaction(Transaction transaction)
        {
            var userId = _userManager.GetUserId(User);
            transaction.UserId = userId;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(transaction);
        }

        // ✅ Update transaction
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction updatedTransaction)
        {
            var userId = _userManager.GetUserId(User);
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null) return NotFound();

            transaction.Type = updatedTransaction.Type;
            transaction.Amount = updatedTransaction.Amount;
            transaction.Date = updatedTransaction.Date;
            transaction.Category = updatedTransaction.Category;
            transaction.Notes = updatedTransaction.Notes;

            await _context.SaveChangesAsync();
            return Ok(transaction);
        }

        // ✅ Delete transaction
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userId = _userManager.GetUserId(User);
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null) return NotFound();

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Transaction deleted" });
        }
    }
}
