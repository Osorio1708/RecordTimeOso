using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using RecordTimeOso.Functions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RecordTimeOso.Tests.Helpers
{
    class MockCloudTableConsolidated : CloudTable
    {
        public MockCloudTableConsolidated(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockCloudTableConsolidated(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableConsolidated(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }

    }
}
