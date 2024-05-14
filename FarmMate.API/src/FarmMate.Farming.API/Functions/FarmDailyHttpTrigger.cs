using System.Net;
using System.Net.Mime;
using FarmMate.Common.Exceptions;
using FarmMate.Common.Validators;
using FarmMate.Farming.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace FarmMate.Farming.API.Functions;

public static class FarmDailyHttpTrigger
{
    private const string FarmDailyRoute = "farms/{farmId}/daily";
    
    [Function(nameof(GetFarmDailyActivity))]
    [OpenApiOperation(operationId: nameof(GetFarmDailyActivity), tags: ["Farm API"], Summary = "Get Farm Daily Activity", Description = "Gets all the farm daily activities belongs to specified farm", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "farmId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Farm Id")]
    [OpenApiParameter(name: "day", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "Day")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Summary = "Farm Daily Activity Object", Description = "This returns the farm daily activity belongs to specified farm and day")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Farm Daily Activity Not Found", Description = "No farm daily activity found for the specified farm and day")]
    public static FarmDaily GetFarmDailyActivity(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{FarmDailyRoute}/{{day}}")] 
        HttpRequestData req,
        [SqlInput($"select * from {Constants.FarmDailyTableName} where [farmId] = @farmId AND [day] = @day", 
            Constants.ConnectionStringSetting, parameters: "@farmId={farmId}")] IReadOnlyCollection<FarmDaily> farmDailyActivity)
    {
        var farmDaily = farmDailyActivity.FirstOrDefault();
        if (farmDaily == null) throw new NotFoundException();

        return farmDaily;
    }
    
    [Function(nameof(CreateFarmDailyActivity))]
    [SqlOutput(Constants.FarmDailyTableName, Constants.ConnectionStringSetting)]
    [OpenApiOperation(operationId: nameof(CreateFarmDailyActivity), tags: ["Farm API"], Summary = "Create Farm Daily Activity", Description = "Create a farm daily activity", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(FarmDaily), Required = true, Description = "Farm daily activity object that needs to be created")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Summary = "Farm Daily Activity Object", Description = "This returns the created farm daily activity")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad Request", Description = "Invalid farm daily activity object")]
    public static async Task<FarmDaily> CreateFarmDailyActivity(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FarmDailyRoute)] HttpRequestData req)
    {
        var farmDaily = await req.ReadFromJsonAsync<FarmDaily>();
        if (farmDaily == null) throw new BadRequestException();

        farmDaily.Id = Guid.NewGuid();
        farmDaily.CreatedDateTime = farmDaily.UpdatedDateTime = DateTime.UtcNow;
        
        if (!farmDaily.Validate().IsValid) throw new BadRequestException();
        return farmDaily;
    }
    
    [Function(nameof(UpdateFarmDailyActivity))]
    [SqlOutput(Constants.FarmDailyTableName, Constants.ConnectionStringSetting)]
    [OpenApiOperation(operationId: nameof(UpdateFarmDailyActivity), tags: ["Farm API"], Summary = "Update Farm Daily Activity", Description = "Update a farm daily activity", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "farmId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Farm Id")]
    [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(FarmDaily), Required = true, Description = "Farm daily activity object that needs to be updated")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Summary = "Farm Daily Activity Object", Description = "This returns the updated farm daily activity")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad Request", Description = "Invalid farm daily activity object")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Farm Daily Activity Not Found", Description = "No farm daily activity found for the specified farm and day")]
    public static async Task<FarmDaily> UpdateFarmDailyActivity(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = $"{FarmDailyRoute}/{{day}}")] HttpRequestData req,
        [SqlInput($"select * from {Constants.FarmDailyTableName} where [farmId] = @farmId AND [day] = @day", 
            Constants.ConnectionStringSetting, parameters: "@farmId={farmId},@day={day}")] IReadOnlyCollection<FarmDaily> farmDailyActivity)
    {
        var farmDailyReq = await req.ReadFromJsonAsync<FarmDaily>();
        if (farmDailyReq == null) throw new BadRequestException();
        
        var farmDaily = farmDailyActivity.FirstOrDefault();
        if (farmDaily == null) throw new NotFoundException();
        
        farmDailyReq.Day = farmDaily.Day;
        farmDailyReq.FarmId = farmDaily.FarmId;
        farmDailyReq.CreatedDateTime = farmDaily.CreatedDateTime;
        farmDailyReq.UpdatedDateTime = DateTime.UtcNow;
        return farmDailyReq;
    }
    
    [Function(nameof(DeleteFarmDailyActivity))]
    [SqlOutput(Constants.FarmDailyTableName, Constants.ConnectionStringSetting)]
    [OpenApiOperation(operationId: nameof(DeleteFarmDailyActivity), tags: ["Farm API"], Summary = "Delete Farm Daily Activity", Description = "Delete a farm daily activity", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "farmId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Farm Id")]
    [OpenApiParameter(name: "day", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "Day")]
    public static IActionResult DeleteFarmDailyActivity(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = $"{FarmDailyRoute}/{{day}}")] HttpRequestData req,
        [SqlInput($"delete from {Constants.FarmDailyTableName} where [farmId] = @farmId AND [day] = @day", 
            Constants.ConnectionStringSetting, parameters: "@farmId={farmId},@day={day}")] IReadOnlyCollection<FarmDaily> farmDailyActivity)
    {
        return new OkResult();
    }
}
