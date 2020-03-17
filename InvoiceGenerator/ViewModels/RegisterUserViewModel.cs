using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceGenerator.ViewModels
{
    public class RegisterUserViewModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required ")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required ")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Organisation Name")]
        [Required(ErrorMessage = " Organisation name is required ")]
        public string OrganisationName { get; set; }

        [Display(Name = "Organisation email")]
        [Required(ErrorMessage = " Organisation email is required ")]   
        public string OrganisationEmail { get; set; }

        [Display(Name = "Organisation address")]
        [Required(ErrorMessage = " address is required ")]
        public string AddressL1 { get; set; }
        public string AddressL2 { get; set; }
        public string AddressL3 { get; set; }
        public string AddressL4 { get; set; }
        public string AddressL5 { get; set; }

        [Display(Name = "Organisation address code")]
        [Required(ErrorMessage = "Address  code is required ")]
        public string AddressCode { get; set; }
        public string Message { get; set; }
    }
}
