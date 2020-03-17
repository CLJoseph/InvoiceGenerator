using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceGenerator.Data.Entities
{
    public enum status {Open,Sent,Overdue, PartPaid,Paid,Cancelled}

    public class tblInvoiceItems 
    {
        public Guid Id { get; set; }
        public string Item { get; set; }
        public string Price { get; set; }
        public string Tax { get; set; }
        public string Total { get; set; }
    }


    public class tblInvoice:LineId
    {
        public string Code { get; set; }
        public status Status { get; set; }     
        public string Date { get; set; } 
        public string Note { get; set; }
        public string From { get; set; }
        public string AttentionOf { get; set; }
        public string To { get; set; }
        public string ToEmail { get; set; }      
        public string Price { get; set; }
        public string Tax { get; set; }
        public string Total { get; set; }
        // navigation properties   
        public Guid ApplicationUserId { get; set; }       
        public List<tblInvoiceItems> Items { get; set; }

    }
}
