using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using RecordTimeOso.Common.Responses;
using RecordTimeOso.Functions.Entities;
using System;
using System.Collections;
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
            log.LogInformation($"New Consolidated requested. time: {DateTime.Now.TimeOfDay.ToString()}");
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
                        if (RT.IdEmployee == RTaux.IdEmployee && !RTaux.Consolidated)
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
                    TableQuery<ConsolidatedEntity> queryC = new TableQuery<ConsolidatedEntity>();
                    TableQuerySegment<ConsolidatedEntity> consolidated = await ConsolidatedTable.ExecuteQuerySegmentedAsync(queryC, null);
                    int sw = 0;

                    foreach (ConsolidatedEntity CE in consolidated)
                    {
                        if (CE.EndTime.Day == endTime.TimeRecorded.Day && CE.IdEmployee == endTime.IdEmployee)
                        {
                            CE.DiffTime = (TimeSpan.Parse(CE.DiffTime) + TP).ToString();
                            CE.WorkedMinutes = CE.WorkedMinutes + TP.TotalMinutes;
                            CE.EndTime = endTime.TimeRecorded;
                            addOperation = TableOperation.Replace(CE);
                            await ConsolidatedTable.ExecuteAsync(addOperation);
                            sw++;
                        }
                    }
                    if (sw == 0)
                    {
                        ConsolidatedEntity consolidatedEntity = new ConsolidatedEntity
                        {
                            ETag = "*",
                            PartitionKey = "RecordTime",
                            RowKey = Guid.NewGuid().ToString(),
                            IdEmployee = startTime.IdEmployee,
                            WorkedMinutes = TP.TotalMinutes,
                            DiffTime = TP.ToString(),
                            startTime = startTime.TimeRecorded,
                            EndTime = endTime.TimeRecorded
                        };
                        addOperation = TableOperation.Insert(consolidatedEntity);
                        await ConsolidatedTable.ExecuteAsync(addOperation);
                    }
                }
            }
            log.LogInformation($"Consolidated Finish. Time: {DateTime.Now.TimeOfDay.ToString()}");
            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = $"The Consolidated  Succesfuly",
                Result = null
            });
        }
        [FunctionName(nameof(GetConsolidatedByDateAsync))]
        public static async Task<IActionResult> GetConsolidatedByDateAsync(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Consolidated/{date}")] HttpRequest req,
           [Table("Consolidated", Connection = "AzureWebJobsStorage")] CloudTable ConsolidatedTable,
           string date,
           ILogger log)
        {
            DateTime dateStart = DateTime.Parse(date + " 00:00");
            DateTime dateEnd = DateTime.Parse(date + " 23:59");
            log.LogInformation($"Get Consolidte by Date {dateStart} - {dateEnd},  recived. ");
            TableQuery<ConsolidatedEntity> query = new TableQuery<ConsolidatedEntity>();
            TableQuerySegment<ConsolidatedEntity> consolidatedEntityTable = await ConsolidatedTable.ExecuteQuerySegmentedAsync(query, null);
            ArrayList result = new ArrayList();
            foreach (ConsolidatedEntity CE in consolidatedEntityTable)
            {
                if (CE.EndTime > dateStart && CE.EndTime < dateEnd)
                {
                    result.Add(CE);
                }
            }
            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = $"The Consolidated  of Date {date} are:",
                Result = result
            });
        }
    }
}
