using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using RecordTimeOso.Common.Models;
using RecordTimeOso.Functions.Entities;
using System;
using System.IO;

namespace RecordTimeOso.Tests.Helpers
{
    public class TestFactory
    {
        public static RecordTimeEntity GetRecordTimeEntity()
        {
            return new RecordTimeEntity
            {
                ETag = "*",
                PartitionKey = "RecordTime",
                RowKey = Guid.NewGuid().ToString(),
                IdEmployee = 1,
                TimeRecorded = DateTime.UtcNow,
                RecordTipe = 0,
                Consolidated = false
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid RecordTimeId, RecordTime RecordTimeRequest)
        {
            string request = JsonConvert.SerializeObject(RecordTimeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{RecordTimeId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid RecordTimeId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{RecordTimeId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(RecordTime RecordTimeRequest)
        {
            string request = JsonConvert.SerializeObject(RecordTimeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static RecordTime GetRecordTime()
        {
            return new RecordTime
            {
                IdEmployee = 1,
                TimeRecorded = DateTime.UtcNow,
                RecordTipe = 0,
                Consolidated = false
            };
        }

        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }
            return logger;
        }
    }
}
