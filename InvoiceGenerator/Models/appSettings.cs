using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceGenerator.Models
{
    public class AppSettings
    {       
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
        public string Enviroment { get; set; }
        public string DatabaseConnection { get; set; }
        public string Test { get; set; }
    }
}
