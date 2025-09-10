using Microsoft.AspNetCore.Identity;

namespace FinanceManager.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        
        public string FullName { get; set; } = string.Empty;
    }
}
