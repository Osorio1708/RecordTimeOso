using System;
using System.Collections.Generic;
using System.Text;

namespace RecordTimeOso.Common.Models
{
    public class RecordTime
    {
        public int IdEmployee { get; set; }
        public DateTime TimeRecorded { get; set; }
        public int RecordTipe { get; set; }
        public bool CoConsolidated { get; set; }
    }
}
