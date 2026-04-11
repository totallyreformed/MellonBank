using System.ComponentModel.DataAnnotations;

namespace MellonBank.ViewModels
{
    public class TransferViewModel
    {
        [Required(ErrorMessage = "Amount is Required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        [Display(Name = "Amount (EUR)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Destination Account Number is Required")]
        [Display(Name = "Destination Account Number")]
        public string DestinationAccountNumber { get; set; }
    }
}
