using System.ComponentModel.DataAnnotations;

namespace MellonBank.ViewModels
{
    public class TransferViewModel
    {
        [Required(ErrorMessage = "Amount is Required")]
        [Range(typeof(decimal), "0.01", "9999999999999999.99", ErrorMessage = "Amount must be between 0.01 and 9,999,999,999,999,999.99")]
        [Display(Name = "Amount (EUR)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Destination Account Number is Required")]
        [Display(Name = "Destination Account Number")]
        public string DestinationAccountNumber { get; set; }
    }
}