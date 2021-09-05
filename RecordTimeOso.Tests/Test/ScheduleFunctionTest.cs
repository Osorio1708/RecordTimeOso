using RecordTimeOso.Functions.Functions;
using RecordTimeOso.Tests.Helpers;
using System;
using Xunit;

namespace RecordTimeOso.Tests.Test
{
    public class ScheduleFunctionTest
    {

        [Fact]

        public async void SheduleFunction_Should_Write_On_Log_Ok_MessageAsync()
        {
            //arrange

            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            MockCloudTableRecordTime mockCloudTableRecordTime = new MockCloudTableRecordTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableConsolidated mockCloudTableConsolidated = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));

            //act

            await ScheduleFunction.RunScheduleConsolidated(null, mockCloudTableRecordTime, mockCloudTableConsolidated, logger);
            string message = logger.Logs[1];

            //assert

            Assert.Contains("Consolidated Finish.", message);

        }


    }
}
