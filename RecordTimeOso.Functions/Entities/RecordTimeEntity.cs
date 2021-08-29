using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace RecordTimeOso.Functions.Entities
{
    public class RecordTimeEntity : TableEntity
    {
        public int IdEmployee { get; set; }
        public DateTime TimeRecorded { get; set; }
        public int RecordTipe { get; set; }
        public bool Consolidated { get; set; }
    }
}