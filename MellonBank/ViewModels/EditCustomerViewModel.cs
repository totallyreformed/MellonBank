using System.ComponentModel.DataAnnotations;

namespace MellonBank.ViewModels
{
    public class EditCustomerViewModel
    {
        [Required(ErrorMessage = "Name is Required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Last Name is Required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone is Required")]
        [Phone]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "AFM is Required")]
        [Display(Name = "AFM")]
        public string AFM { get; set; }
    }
}
