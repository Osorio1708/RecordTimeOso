using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using RecordTimeOso.Functions.Entities;
using System;
using System.Linq;
using System.Reflection;
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

        public override async Task<TableQuerySegment<RecordTimeEntity>> ExecuteQuerySegmentedAsync<RecordTimeEntity>(TableQuery<RecordTimeEntity> query, TableContinuationToken token)
        {
            ConstructorInfo constructor = typeof(TableQuerySegment<RecordTimeEntity>)
                   .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                   .FirstOrDefault(c => c.GetParameters().Count() == 1);

            return await Task.FromResult(constructor.Invoke(new object[] { TestFactory.GetRecordTimeEntities() }) as TableQuerySegment<RecordTimeEntity>);
        }
    }
}
