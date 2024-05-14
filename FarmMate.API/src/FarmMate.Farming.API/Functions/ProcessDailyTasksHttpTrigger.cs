using System.Net;
using System.Net.Mime;
using FarmMate.Common.Exceptions;
using FarmMate.Farming.API.Entities;
using FarmMate.Farming.API.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace FarmMate.Farming.API.Functions;

public class ProcessDailyTasksHttpTrigger(IOpenAIService openAiService)
{
    private const string FarmDailyTasksRoute = "farms/{farmId}/processDailyTasks";
    
    [Function(nameof(ProcessDailyTasks))]
    [SqlOutput(Constants.FarmDailyTaskTableName, Constants.ConnectionStringSetting)]
    [OpenApiOperation(operationId: nameof(ProcessDailyTasks), tags: ["Farm API"],
        Summary = "Process Farm Daily Tasks", Description = "Process farm daily tasks",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "farmId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Farm Id")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: MediaTypeNames.Application.Json,
        bodyType: typeof(string), Summary = "Farm Daily Tasks",
        Description = "This returns the created farm daily tasks")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad Request",
        Description = "Invalid Request")]
    public async Task<FarmDailyTask[]> ProcessDailyTasks(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FarmDailyTasksRoute)]
        HttpRequestData req, Guid farmId,
        [SqlInput($"select * from {Constants.FarmsTableName} where [id] = @farmId", 
            Constants.ConnectionStringSetting, parameters: "@farmId={farmId}")] IReadOnlyCollection<Farm> farms)
    {
        var farm = farms?.FirstOrDefault();
        if (farm == null) throw new BadRequestException();

        var pages = (int)Math.Ceiling(Convert.ToDecimal(farm.HarvestInDays / 10));

        var tasks = new List<Task<DailyTasksResponse>>();
        for (var i = 0; i < pages; i++)
        {
            tasks.Add(GetOpenAiResponse(farm, i));
        }

        // Wait for all tasks to complete
        var results = await Task.WhenAll(tasks);

        // Process all responses
        var farmDailyTasks = results.SelectMany(result => 
            result.Tasks.SelectMany(dailyItem =>
                dailyItem.Todos?.Select(todo => new FarmDailyTask
                {
                    Id = Guid.NewGuid(),
                    FarmDailyId = Guid.NewGuid(),
                    FarmId = farmId,
                    Day = dailyItem.Day,
                    Description = todo,
                    IsCompleted = false,
                    CreatedDateTime = DateTime.UtcNow,
                    UpdatedDateTime = DateTime.UtcNow
                }) ?? Enumerable.Empty<FarmDailyTask>()
            )
        ).ToList();

        Console.WriteLine(JsonConvert.SerializeObject(farmDailyTasks));
        return farmDailyTasks.ToArray();
    }
    
    private async Task<DailyTasksResponse> GetOpenAiResponse(Farm farm, int pageIndex)
    {
        var completionResult = await openAiService.ChatCompletion.CreateCompletion(
            new ChatCompletionCreateRequest
            {
                ResponseFormat = new ResponseFormat {Type = StaticValues.CompletionStatics.ResponseFormat.Json},
                Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem(Constants.DailyTasksSystemRolePrompt),
                    ChatMessage.FromSystem(Constants.DailyTasksConversationTypePrompt),
                    ChatMessage.FromSystem(Constants.DailyTasksResponseFormatPrompt),
                    ChatMessage.FromUser(string.Format(Constants.DailyTasksUserPrompt, 
                        farm.Crop, 
                        farm.HarvestInDays, 
                        pageIndex * 10 + 1,
                        (pageIndex + 1) * 10))
                }
            });

        return (completionResult.Successful ? 
            JsonConvert.DeserializeObject<DailyTasksResponse>(completionResult.Choices.First().Message.Content!) : 
            new DailyTasksResponse { Tasks = new List<DailyItem>().ToArray() })!;
    }
}
