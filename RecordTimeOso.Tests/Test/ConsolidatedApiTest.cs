using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecordTimeOso.Functions.Entities;
using RecordTimeOso.Functions.Functions;
using RecordTimeOso.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RecordTimeOso.Tests.Test
{
    public class ConsolidatedApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();
        [Fact]
        public async Task GetConsolidatedByDate_Should_Return_200Async()
        {

            //arrange
            string Date = "2021-08-16";
            MockCloudTableConsolidated mockTable = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(Date);
            //act
            IActionResult response = await ConsolidatedAPI.GetConsolidatedByDateAsync(request, mockTable, Date, logger);
            //assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
