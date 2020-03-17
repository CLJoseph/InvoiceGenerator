using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceGenerator.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required ")]
        public string Person { get; set; }
       
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
