using System;

namespace RecordTimeOso.Common.Models
{
    public class RecordTime
    {
        public int IdEmployee { get; set; }
        public DateTime TimeRecorded { get; set; }
        public int RecordTipe { get; set; }
        public bool Consolidated { get; set; }
    }
}
