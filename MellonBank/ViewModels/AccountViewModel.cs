using System.ComponentModel.DataAnnotations;
using MellonBank.Areas.Identity.Data;

namespace MellonBank.ViewModels
{
    public class AccountViewModel
    {
        [Required(ErrorMessage = "Customer AFM is Required")]
        [Display(Name = "Customer AFM")]
        public string CustomerAFM { get; set; }

        [Required(ErrorMessage = "Balance is Required")]
        [Display(Name = "Balance (EUR)")]
        public decimal Balance { get; set; }

        [Required(ErrorMessage = "Account Number is Required")]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Branch is Required")]
        [Display(Name = "Branch")]
        public string Branch { get; set; }

        [Required(ErrorMessage = "Account Type is Required")]
        [Display(Name = "Account Type")]
        public AccountType Type { get; set; }
    }
}
