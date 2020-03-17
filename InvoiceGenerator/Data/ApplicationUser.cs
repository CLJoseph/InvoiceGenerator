using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceGenerator.Data
{
    public class ApplicationUser:IdentityUser<Guid>
    {
        public string Person { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationEmail { get; set; }
        public string AddressL1 { get; set; }
        public string AddressL2 { get; set; }
        public string AddressL3 { get; set; }
        public string AddressL4 { get; set; }
        public string AddressL5 { get; set; }
        public string AddressCode { get; set; }
    }
}
