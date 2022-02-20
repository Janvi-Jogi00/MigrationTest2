using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class DestinationTable
    {
        public int Id { get; set; }
        public int Sum { get; set; }
        public int SourceTableId { get; set; }
        public SourceTable SourceTable { get; set; }
    }
}
