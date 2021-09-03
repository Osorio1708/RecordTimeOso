using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecordTimeOso.Common.Models;
using RecordTimeOso.Functions.Functions;
using RecordTimeOso.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RecordTimeOso.Tests.Test
{
    public class RecordTimeApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateTodo_Should_Return_200()
        {
            // Arrange
            MockCloudTableRecordTime mockRecordTime = new MockCloudTableRecordTime(new Uri(""));
            RecordTime recordTimeRequest = TestFactory.GetRecordTimeRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(recordTimeRequest);
            //Act
            IActionResult response = await RecordTimeAPI.CreateRecordTime(request, mockRecordTime, logger);
            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

    }
}
