using System.Net;
using System.Net.Mime;
using FarmMate.Common.Exceptions;
using FarmMate.Farming.API.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace FarmMate.Farming.API.Functions;

public static class FarmDailyTasksHttpTrigger
{
    private const string FarmDailyTasksRoute = "farms/{farmId}/daily/{day}/tasks";
    
    [Function(nameof(GetFarmDailyTasks))]
    [OpenApiOperation(operationId: nameof(GetFarmDailyTasks), tags: ["Farm API"], Summary = "Get Farm Daily Tasks", Description = "Gets all the farm daily tasks belongs to specified farm and day", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "farmId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Farm Id")]
    [OpenApiParameter(name: "day", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "Day")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Summary = "Farm Daily Tasks", Description = "This returns the farm daily tasks belongs to specified farm and day")]
    public static IReadOnlyCollection<FarmDailyTask> GetFarmDailyTasks(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{FarmDailyTasksRoute}")] 
        HttpRequestData req,
        [SqlInput($"select * from {Constants.FarmDailyTaskTableName} where [farmId] = @farmId AND [day] = @day",
            Constants.ConnectionStringSetting, parameters: "@farmId={farmId},@day={day}")] IReadOnlyCollection<FarmDailyTask> farmDailyTasks)
    {
        return farmDailyTasks;
    }

    [Function(nameof(CreateFarmDailyTasks))]
    [SqlOutput(Constants.FarmDailyTaskTableName, Constants.ConnectionStringSetting)]
    [OpenApiOperation(operationId: nameof(CreateFarmDailyTasks), tags: ["Farm API"],
        Summary = "Create Farm Daily Tasks", Description = "Create a farm daily tasks",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(FarmDailyTask), Required = true,
        Description = "Farm daily tasks object that needs to be created")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: MediaTypeNames.Application.Json,
        bodyType: typeof(string), Summary = "Farm Daily Tasks",
        Description = "This returns the created farm daily tasks")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad Request",
        Description = "Invalid farm daily tasks object")]
    public static async Task<FarmDailyTask[]> CreateFarmDailyTasks(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FarmDailyTasksRoute)]
        HttpRequestData req)
    {
        var farmDailyTasks = await req.ReadFromJsonAsync<FarmDailyTask[]>();
        if (farmDailyTasks == null) throw new BadRequestException();

        foreach (var task in farmDailyTasks)
        {
            task.Id = Guid.NewGuid();
            task.CreatedDateTime = task.UpdatedDateTime = DateTime.UtcNow;
        }
        return farmDailyTasks;
    }
    
    [Function(nameof(UpdateFarmDailyTask))]
    [SqlOutput(Constants.FarmDailyTaskTableName, Constants.ConnectionStringSetting)]
    [OpenApiOperation(operationId: nameof(UpdateFarmDailyTask), tags: ["Farm API"],
        Summary = "Update Farm Daily Task", Description = "Update a farm daily task",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "farmId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Farm Id")]
    [OpenApiParameter(name: "day", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "Day")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Daily Task Id")]
    [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(FarmDailyTask), Required = true,
        Description = "Farm daily task object that needs to be updated")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json,
        bodyType: typeof(string), Summary = "Farm Daily Task",
        Description = "This returns the updated farm daily task")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad Request",
        Description = "Invalid farm daily task object")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Farm Daily Task Not Found",
        Description = "No farm daily task found for the specified farm and day")]
    public static FarmDailyTask UpdateFarmDailyTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = $"{FarmDailyTasksRoute}/{{id}}")]
        HttpRequestData req,
        [SqlInput($"select * from {Constants.FarmDailyTaskTableName} where [farmId] = @farmId AND [day] = @day AND [id] = @id", 
            Constants.ConnectionStringSetting, parameters: "@farmId={farmId},@day={day},@id={id}")] IReadOnlyCollection<FarmDailyTask> farmDailyTasks)
    {
        var status = req.Query.Get("status");
        if (status == null) throw new BadRequestException();
        
        var farmDailyTask = farmDailyTasks.FirstOrDefault();
        if (farmDailyTask == null) throw new NotFoundException();
        
        farmDailyTask.IsCompleted = bool.Parse(status);
        farmDailyTask.UpdatedDateTime = DateTime.UtcNow;
        return farmDailyTask;
    }
}