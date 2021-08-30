using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using RecordTimeOso.Common.Models;
using RecordTimeOso.Common.Responses;
using RecordTimeOso.Functions.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RecordTimeOso.Functions.Functions
{
    public static class RecordTimeAPI
    {
        [FunctionName(nameof(CreateRecordTime))]
        public static async Task<IActionResult> CreateRecordTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "RecordTime")] HttpRequest req,
            [Table("RecordTime", Connection = "AzureWebJobsStorage")] CloudTable RecordTimeTable,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            RecordTime recordTime = JsonConvert.DeserializeObject<RecordTime>(requestBody);
            log.LogInformation("New Record Time recieved.");

            if (recordTime.IdEmployee <= 0)
            {
                return new BadRequestObjectResult(new Response
                {

                    IsSuccess = false,
                    Message = "The IdEmployee must be positive and greater than 0"

                });
            }

            if (recordTime.RecordTipe != 0 && recordTime.RecordTipe != 1)
            {
                return new BadRequestObjectResult(new Response
                {

                    IsSuccess = false,
                    Message = $"The RecordTipe must be 0 or 1"

                });
            }

            /*
            TableQuery<RecordTimeEntity> query = new TableQuery<RecordTimeEntity>();
            TableQuerySegment<RecordTimeEntity> tableRecordTime = await RecordTimeTable.ExecuteQuerySegmentedAsync(query, null);
            foreach (RecordTimeEntity RT in tableRecordTime)
            {
                if (RT.IdEmployee == recordTime.IdEmployee && RT.RecordTipe == recordTime.RecordTipe)
                {
                    return new BadRequestObjectResult(new Response
                    {

                        IsSuccess = false,
                        Message = "This record has already been taken"

                    });
                }
            }
            */

            RecordTimeEntity recordTimeEntity = new RecordTimeEntity
            {

                ETag = "*",
                PartitionKey = "RecordTime",
                RowKey = Guid.NewGuid().ToString(),

                IdEmployee = recordTime.IdEmployee,
                TimeRecorded = recordTime.TimeRecorded,
                RecordTipe = recordTime.RecordTipe,
                Consolidated = false,

            };

            TableOperation addOperation = TableOperation.Insert(recordTimeEntity);
            await RecordTimeTable.ExecuteAsync(addOperation);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = "Record Time Saved",
                Result = recordTime
            });

        }

        [FunctionName(nameof(UpdateRecordTime))]
        public static async Task<IActionResult> UpdateRecordTime(
         [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "RecordTime/{id}")] HttpRequest req,
         [Table("RecordTime", Connection = "AzureWebJobsStorage")] CloudTable RecordTimeTable,
         string id,
         ILogger log)
        {
            log.LogInformation($"Update for RecordTime: {id}, received");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            RecordTime recordTime = JsonConvert.DeserializeObject<RecordTime>(requestBody);

            TableOperation findOperation = TableOperation.Retrieve<RecordTimeEntity>("RecordTime", id);
            TableResult findResult = await RecordTimeTable.ExecuteAsync(findOperation);
            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {

                    IsSuccess = false,
                    Message = "Record Time not found."

                });
            }

            if (recordTime.IdEmployee <= 0)
            {
                return new BadRequestObjectResult(new Response
                {

                    IsSuccess = false,
                    Message = "The IdEmployee must be positive and greater than 0"

                });
            }
            if (recordTime.RecordTipe != 0 && recordTime.RecordTipe != 1)
            {
                return new BadRequestObjectResult(new Response
                {

                    IsSuccess = false,
                    Message = "The RecordTipe must be 0 or 1"

                });
            }
            RecordTimeEntity recordTimeEntity = (RecordTimeEntity)findResult.Result;
            recordTimeEntity.IdEmployee = recordTime.IdEmployee;
            recordTimeEntity.RecordTipe = recordTime.RecordTipe;
            recordTimeEntity.TimeRecorded = recordTime.TimeRecorded;
            TableOperation addOperation = TableOperation.Replace(recordTimeEntity);
            await RecordTimeTable.ExecuteAsync(addOperation);

            string message = $"Record Time: {id}, updated in table.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = recordTime
            });
        }

        [FunctionName(nameof(GetAllRecordTime))]
        public static async Task<IActionResult> GetAllRecordTime(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "RecordTime")] HttpRequest req,
          [Table("RecordTime", Connection = "AzureWebJobsStorage")] CloudTable RecordTimeTable,
          ILogger log)
        {
            log.LogInformation("Get all Record Time received.");

            TableQuery<RecordTimeEntity> query = new TableQuery<RecordTimeEntity>();
            TableQuerySegment<RecordTimeEntity> recordTimeEntity = await RecordTimeTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieved all Record Time.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = recordTimeEntity
            });
        }

        [FunctionName(nameof(GetRecordTimeById))]
        public static IActionResult GetRecordTimeById(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "RecordTime/{id}")] HttpRequest req,
           [Table("RecordTime", "RecordTime", "{id}", Connection = "AzureWebJobsStorage")] RecordTimeEntity recordTimeEntity,
           string id,
           ILogger log)
        {
            log.LogInformation($"Get ToDo by id {id},  recived. ");

            if (recordTimeEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {

                    IsSuccess = false,
                    Message = "Record Time not found."

                });
            }

            string message = $"Record Time: {id}, retrieved.";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = recordTimeEntity
            });
        }

        [FunctionName(nameof(DeleteRecordTime))]
        public static async Task<IActionResult> DeleteRecordTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "RecordTime/{id}")] HttpRequest req,
            [Table("RecordTime", "RecordTime", "{id}", Connection = "AzureWebJobsStorage")] RecordTimeEntity recordTimeEntity,
            [Table("RecordTime", Connection = "AzureWebJobsStorage")] CloudTable RecordTimeTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Delete ToDo: {id}, recived.");

            if (recordTimeEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {

                    IsSuccess = false,
                    Message = "Record Time not found."

                });
            }

            await RecordTimeTable.ExecuteAsync(TableOperation.Delete(recordTimeEntity));
            string message = $"Record Time: {id}, deleted.";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = recordTimeEntity
            });
        }
    }
}
