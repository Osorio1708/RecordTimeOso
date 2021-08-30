using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using RecordTimeOso.Common.Responses;
using RecordTimeOso.Functions.Entities;
using System;
using System.Threading.Tasks;

namespace RecordTimeOso.Functions.Functions
{
    public static class ConsolidatedAPI
    {
        [FunctionName(nameof(GenerateConsolidated))]
        public static async Task<IActionResult> GenerateConsolidated(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Consolidated")] HttpRequest req,
            [Table("RecordTime", Connection = "AzureWebJobsStorage")] CloudTable RecordTimeTable,
            [Table("Consolidated", Connection = "AzureWebJobsStorage")] CloudTable ConsolidatedTable,
            ILogger log)
        {

            TableQuery<RecordTimeEntity> query = new TableQuery<RecordTimeEntity>();
            TableQuerySegment<RecordTimeEntity> tableRecordTime = await RecordTimeTable.ExecuteQuerySegmentedAsync(query, null);
            log.LogInformation("New Consolidated requested.");
            foreach (RecordTimeEntity RT in tableRecordTime)
            {
                RecordTimeEntity startTime = null;
                RecordTimeEntity endTime = null;
                int switchStart = 0;
                int switchEnd = 0;
                if (!RT.Consolidated)
                {
                    foreach (RecordTimeEntity RTaux in tableRecordTime)
                    {
                        if (RT.IdEmployee == RTaux.IdEmployee)
                        {
                            if (RTaux.RecordTipe == 0 && switchStart == 0)
                            {
                                startTime = RTaux;
                                switchStart++;
                            }
                            if (RTaux.RecordTipe == 1 && switchEnd == 0)
                            {
                                endTime = RTaux;
                                switchEnd++;
                            }
                        }
                    }
                }
                if (startTime != null && endTime != null)
                {
                    TableOperation addOperation;
                    TimeSpan TP = endTime.TimeRecorded - startTime.TimeRecorded;
                   
                    endTime.Consolidated = true;
                    startTime.Consolidated = true;
                    addOperation = TableOperation.Replace(endTime);
                    await RecordTimeTable.ExecuteAsync(addOperation);
                    addOperation = TableOperation.Replace(startTime);
                    await RecordTimeTable.ExecuteAsync(addOperation);
                    ConsolidatedEntity consolidatedEntity = new ConsolidatedEntity
                    {
                        ETag = "*",
                        PartitionKey = "RecordTime",
                        RowKey = Guid.NewGuid().ToString(),
                        IdEmployee = startTime.IdEmployee,
                        WorkedHours = TP.Hours,
                        DiffTime = TP.ToString(),
                        startTime = startTime.TimeRecorded,
                        EndTime = endTime.TimeRecorded
                    };
                    addOperation = TableOperation.Insert(consolidatedEntity);
                    await ConsolidatedTable.ExecuteAsync(addOperation);
                    log.LogInformation("Consolidated Finish.");
                }
            }
            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = "Consolidated Succesfuly"
            });
        }
    }
}