using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceGenerator.Data.Entities
{
    public class LineId
    {
        // this is the line identifier (primary Key) in a table
        // using a Guid as this lifts the responsibility of generating
        // this value from the database.   
        public Guid Id { get; set; }
        // row version control
        // each time a line is modified the database will record that time.
        // useful for checking that data has not changed between a read and write
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
