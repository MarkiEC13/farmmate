using System.Data;
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

public static class FarmHttpTrigger
{
    private const string FarmsRoute = "farms";
    
    [Function(nameof(GetFarmsByUserId))]
    [OpenApiOperation(operationId: nameof(GetFarmsByUserId), tags: ["Farm API"], Summary = "Get Farms By User Id", Description = "Gets all the farms belongs to specified user", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "userId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "User Id")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Summary = "Farm Objects", Description = "This returns all the farms belongs to specified user")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "No Farms Found", Description = "No farms found for the specified user")]
    public static IReadOnlyCollection<Farm> GetFarmsByUserId(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"users/{{userId}}/{FarmsRoute}")] 
        HttpRequestData req,
        [SqlInput($"select * from {Constants.FarmsTableName} where [userId] = @userId", 
            Constants.ConnectionStringSetting, parameters: "@userId={userId}")] IReadOnlyCollection<Farm> farms)
    {
        if (farms.Count == 0) throw new NotFoundException();
        return farms;
    }
    
    [Function(nameof(GetFarmById))]
    [OpenApiOperation(operationId: nameof(GetFarmById), tags: ["Farm API"], Summary = "Get Farm By Id", Description = "Gets the farm by specified id", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Farm Id")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Summary = "Farm Object", Description = "This returns the farm by specified id")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Farm Not Found", Description = "No farm found for the specified id")]
    public static Farm GetFarmById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{FarmsRoute}/{{id}}")] HttpRequestData req,
        [SqlInput($"select * from {Constants.FarmsTableName} where [id] = @id",
            Constants.ConnectionStringSetting, parameters: "@id={id}")] IReadOnlyCollection<Farm> farms)
    {
        var farm = farms.FirstOrDefault();
        if (farm == null) throw new NotFoundException();

        return farm;
    }
    
    [Function(nameof(CreateFarm))]
    [SqlOutput(Constants.FarmsTableName, Constants.ConnectionStringSetting)]
    [OpenApiOperation(operationId: nameof(CreateFarm), tags: ["Farm API"], Summary = "Create Farm", Description = "Create a farm", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(Farm), Required = true, Description = "Farm object that needs to be created")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Summary = "Farm Object", Description = "This returns the created farm")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad Request", Description = "Invalid farm object")]
    public static async Task<Farm> CreateFarm(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FarmsRoute)] HttpRequestData req)
    {
        var farm = await req.ReadFromJsonAsync<Farm>();
        if (farm == null) throw new BadRequestException();
        
        farm.Id = Guid.NewGuid();
        farm.CreatedDateTime = farm.UpdatedDateTime = DateTime.UtcNow;
        
        if (!farm.Validate().IsValid) throw new BadRequestException();
        return farm;
    }
    
    [Function(nameof(UpdateFarm))]
    [SqlOutput(Constants.FarmsTableName, Constants.ConnectionStringSetting)]
    [OpenApiOperation(operationId: nameof(UpdateFarm), tags: ["Farm API"], Summary = "Update Farm", Description = "Update a farm", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Farm Id")]
    [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(Farm), Required = true, Description = "Farm object that needs to be updated")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Summary = "Farm Object", Description = "This returns the updated farm")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad Request", Description = "Invalid farm object")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Farm Not Found", Description = "No farm found for the specified id")]
    public static async Task<Farm> UpdateFarm(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = $"{FarmsRoute}/{{id}}")] 
            HttpRequestData req, Guid id,
        [SqlInput($"select * from {Constants.FarmsTableName} where [Id] = @id", 
            Constants.ConnectionStringSetting, parameters: "@id={id}")] IReadOnlyCollection<Farm> farms)
    {
        var farmToBeUpdated = await req.ReadFromJsonAsync<Farm>();
        if (farmToBeUpdated == null) throw new BadRequestException();

        var farm = farms.FirstOrDefault();
        if (farm == null) throw new NotFoundException();

        farmToBeUpdated.Id = id;
        farmToBeUpdated.CreatedDateTime = farm.CreatedDateTime;
        farmToBeUpdated.UpdatedDateTime = DateTime.UtcNow;
        
        if (!farmToBeUpdated.Validate().IsValid) throw new BadRequestException();
        return farmToBeUpdated;
    }
    
    [Function(nameof(DeleteFarm))]
    [SqlOutput(Constants.FarmsTableName, Constants.ConnectionStringSetting)]
    [OpenApiOperation(operationId: nameof(DeleteFarm), tags: ["Farm API"], Summary = "Delete Farm", Description = "Delete a farm", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Farm Id")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Summary = "Farm Deleted", Description = "This returns OK if the farm is deleted")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Farm Not Found", Description = "No farm found for the specified id")]
    public static IActionResult DeleteFarm(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = $"{FarmsRoute}/{{id}}")] 
            HttpRequestData req, Guid id,
        [SqlInput($"delete from {Constants.FarmsTableName} where [Id] = @id",
            Constants.ConnectionStringSetting, parameters: "@id={id}", commandType: CommandType.Text)] IReadOnlyCollection<Farm> farms)
    {
        return new OkResult();
    }
}