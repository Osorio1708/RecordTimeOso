using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecordTimeOso.Common.Models;
using RecordTimeOso.Functions.Entities;
using RecordTimeOso.Functions.Functions;
using RecordTimeOso.Tests.Helpers;
using System;
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
            MockCloudTableRecordTime mockRecordTime = new MockCloudTableRecordTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            RecordTime recordTimeRequest = TestFactory.GetRecordTimeRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(recordTimeRequest);
            //Act
            IActionResult response = await RecordTimeAPI.CreateRecordTime(request, mockRecordTime, logger);
            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void UpdateTodo_Should_Return_200()
        {
            // Arrange
            MockCloudTableRecordTime mockRecordTime = new MockCloudTableRecordTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            RecordTime recordTimeRequest = TestFactory.GetRecordTimeRequest();
            Guid recordTimeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(recordTimeId, recordTimeRequest);
            //Act
            IActionResult response = await RecordTimeAPI.UpdateRecordTime(request, mockRecordTime, recordTimeId.ToString(), logger);
            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        
        [Fact]
        public async void DeleteRegister_Should_Return_200()
        {

            //arrange
            MockCloudTableRecordTime mockTable = new MockCloudTableRecordTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            RecordTime recordTimeRequest = TestFactory.GetRecordTimeRequest();
            RecordTimeEntity recordTimeEntity = TestFactory.GetRecordTimeEntity();
            Guid recordTimeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(recordTimeRequest);

            //act
            IActionResult response = await RecordTimeAPI.DeleteRecordTime(request, recordTimeEntity, mockTable, recordTimeId.ToString(), logger);

            //assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void GetAllRegisters_Should_Return_200()
        {
            //arrange
            MockCloudTableRecordTime mockTable = new MockCloudTableRecordTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            DefaultHttpRequest request = TestFactory.CreateHttpRequest();

            //act
            IActionResult response = await RecordTimeAPI.GetAllRecordTime(request, mockTable, logger);

            //assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public void GetRegisterById_Should_Return_200()
        {

            //arrange
            MockCloudTableRecordTime mockTable = new MockCloudTableRecordTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            RecordTimeEntity recordTimeEntity = TestFactory.GetRecordTimeEntity();
            Guid recordTimeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest();

            //act
            IActionResult response = RecordTimeAPI.GetRecordTimeById(request, recordTimeEntity, recordTimeId.ToString(), logger);

            //assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
