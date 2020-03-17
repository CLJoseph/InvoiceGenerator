using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceGenerator.Data.Entities
{
    public class tblLookup :LineId
    {  
        public string Lookup { get; set; }
        public string Value { get; set; }
        public string Note { get; set; }
    }
}
