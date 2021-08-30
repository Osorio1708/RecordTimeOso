using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace RecordTimeOso.Functions.Entities
{
    public class ConsolidatedEntity : TableEntity
    {
        public int IdEmployee { get; set; }

        public Double WorkedMinutes { get; set; }

        public String DiffTime { get; set; }

        public DateTime startTime { get; set; }

        public DateTime EndTime { get; set; }


    }
}
