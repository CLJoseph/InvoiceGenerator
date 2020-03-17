using InvoiceGenerator.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceGenerator.ViewModels
{
    public class InvoiceItemViewModel
    {
        public string Id { get; set; }
        public string RowVersion { get; set; }
        public string InvoiceId { get; set; }
        public string Item { get; set; }
        [DataType(DataType.Currency)]
        public string Price { get; set; }
        [DataType(DataType.Currency)]
        public string Tax { get; set; }
        [DataType(DataType.Currency)]
        public string Total { get; set; }
    }

    public class InvoiceVewModel
    {
        public InvoiceVewModel()
        {
            Items = new List<InvoiceItemViewModel>();
        }
        public string Id { get; set; }
        public string RowVersion { get; set; }
        public string ApplicationUserId { get; set; }
        [Display(Name = "Code : ")]
        [Required(ErrorMessage = " Invoice code is required ")]       
        public string Code { get; set; }
        public status Status { get; set; }
        public string Date { get; set; }
        public string Note { get; set; }
        public string From { get; set; }
        public string Person { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationEmail { get; set; }
        public string AddressL1 { get; set; }
        public string AddressL2 { get; set; }
        public string AddressL3 { get; set; }
        public string AddressL4 { get; set; }
        public string AddressL5 { get; set; }
        public string AddressCode { get; set; }
        public string InvoiceUrl { get; set; }

        [Display(Name = "Attention of")]
        [Required(ErrorMessage = " Attention Of is required ")]
        public string AttentionOf { get; set; }
        [Display(Name = "To : Organisation Name & Address")]
        [Required(ErrorMessage = " To is required ")]
        public string To { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = " To email is required. ")]
        [EmailAddress(ErrorMessage ="Invalid email address entered.")]
        public string ToEmail { get; set; }
        [DataType(DataType.Currency)]
        public string Price { get; set; }
        [DataType(DataType.Currency)]
        public string Tax { get; set; }
        [DataType(DataType.Currency)]
        public string Total { get; set; }
        public List<InvoiceItemViewModel> Items { get; set; }
    }

    public class InvoiceIndexItemVewModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        [DataType(DataType.Date)]
        public string Date { get; set; }

        public status Status { get; set; }
        public string Total { get; set; }
    }

    public class InvoiceIndexVewModel
    {
        public InvoiceIndexVewModel()
        {
            Items = new List<InvoiceIndexItemVewModel>();
        }        
        public List<InvoiceIndexItemVewModel> Items { get; set; }
        public string Message { get; set; }

    }
}
