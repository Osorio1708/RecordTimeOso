using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace RecordTimeOso.Tests.Helpers
{
    public class MockCloudTableRecordTime : CloudTable
    {
        public MockCloudTableRecordTime(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockCloudTableRecordTime(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableRecordTime(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }

        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = 200,
                Result = TestFactory.GetRecordTimeEntity()
            });
        }
    }
}
