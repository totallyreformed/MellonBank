using System.ComponentModel.DataAnnotations;

namespace MellonBank.ViewModels
{
    public class DepositViewModel
    {
        [Required(ErrorMessage = "Amount is Required")]
        [Range(typeof(decimal), "0.01", "9999999999999999.99", ErrorMessage = "Amount must be between 0.01 and 9,999,999,999,999,999.99")]
        [Display(Name = "Amount (EUR)")]
        public decimal Amount { get; set; }
    }
}