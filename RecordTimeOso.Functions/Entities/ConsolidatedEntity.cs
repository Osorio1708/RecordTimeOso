﻿using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace RecordTimeOso.Functions.Entities
{
    public class ConsolidatedEntity : TableEntity
    {
        public int IdEmployee { get; set; }

        public int WorkedHours { get; set; }

        public TimeSpan diffTime { get; set; }

        public DateTime startTime { get; set; }

        public DateTime EndTime { get; set; }


    }
}
