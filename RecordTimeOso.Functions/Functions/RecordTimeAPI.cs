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

            if (recordTime.RecordTipe != 0 && recordTime.RecordTipe != 1 )
            {
                return new BadRequestObjectResult(new Response
                {

                    IsSuccess = false,
                    Message = $"The RecordTipe must be 0 or 1 {recordTime.RecordTipe}"

                });
            }

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

            RecordTimeEntity recordTimeEntity = new RecordTimeEntity
            {

                ETag = "*",
                PartitionKey = "RecordTime",
                RowKey = Guid.NewGuid().ToString(),

                IdEmployee = recordTime.IdEmployee,
                TimeRecorded = DateTime.UtcNow,
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

    }
}
