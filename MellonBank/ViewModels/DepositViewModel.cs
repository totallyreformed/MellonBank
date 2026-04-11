using System.ComponentModel.DataAnnotations;

namespace MellonBank.ViewModels
{
    public class DepositViewModel
    {
        [Required(ErrorMessage = "Amount is Required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        [Display(Name = "Amount (EUR)")]
        public decimal Amount { get; set; }
    }
}
