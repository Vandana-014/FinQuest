using System.ComponentModel.DataAnnotations;

namespace FinanceManager.API.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty; // Income or Expense

        [Required]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string? Category { get; set; }
        public string? Notes { get; set; }
    }
}
