using System.ComponentModel.DataAnnotations;

namespace FinanceManager.API.Models
{
    public class Badge
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime DateUnlocked { get; set; } = DateTime.UtcNow;
    }
}
