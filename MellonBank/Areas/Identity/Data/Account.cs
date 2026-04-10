using System.ComponentModel.DataAnnotations;

namespace MellonBank.Areas.Identity.Data
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public AccountType Type { get; set; }
        public virtual AppUser User { get; set; } = null!;
    }
}
