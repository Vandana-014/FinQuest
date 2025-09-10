using System.ComponentModel.DataAnnotations;

namespace FinanceManager.API.Models
{
    public class Goal
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public decimal TargetAmount { get; set; }

        public decimal CurrentAmount { get; set; } = 0;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        public string Status { get; set; } = "InProgress"; // InProgress, Completed, Failed
    }
}
