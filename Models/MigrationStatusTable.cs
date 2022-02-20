using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class MigrationStatusTable
    {
        public int Id { get; set; }
        public int StartingNum { get; set; }
        public int EndingNum { get; set; }
        public string Status { get; set; }
    }
}
