using System.Net;
using System.Net.Mime;
using FarmMate.Common.Exceptions;
using FarmMate.Common.Services;
using FarmMate.CropRecommendation.API.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Newtonsoft.Json;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace FarmMate.CropRecommendation.API.Functions;

public class CropRecommendationHttpTrigger(IOpenAIService openAiService, IMLService mlService)
{
    private const string CropRecommendationRoute = "crop-recommendation";

    [Function(nameof(GetCropRecommendation))]
    [OpenApiOperation(operationId: nameof(GetCropRecommendation), tags: ["Crop Recommendation API"], Summary = "Get Crop Recommendation", Description = "Gets crop recommendation based on the input", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(CropRecommendationRequest), Required = true, Description = "Crop recommendation request object")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(CropRecommendationResponse), Summary = "Crop Recommendation Object", Description = "This returns the crop recommendation based on the input")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad Request", Description = "Invalid crop recommendation request object")]
    public async Task<CropRecommendationResponse?> GetCropRecommendation(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = CropRecommendationRoute)] HttpRequestData req)
    {
        var cropRecommendationRequest = await req.ReadFromJsonAsync<CropRecommendationRequest>();
        if (cropRecommendationRequest == null) throw new BadRequestException();

        var cropRecommendationResult = await mlService.GetPredictionAsync(req, "crop-recommendation-model");
        var cropPricingModel = await mlService.GetPredictionAsync(cropRecommendationResult, "crop-pricing-model");

        var completionResult = await openAiService.ChatCompletion.CreateCompletion(
            new ChatCompletionCreateRequest
            {
                ResponseFormat = new ResponseFormat {Type = StaticValues.CompletionStatics.ResponseFormat.Json},
                Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem(Constants.SystemRolePrompt),
                    ChatMessage.FromSystem(Constants.ConversationTypePrompt),
                    ChatMessage.FromSystem(Constants.CropRecommendationResponseFormatPrompt),
                    ChatMessage.FromSystem(JsonConvert.SerializeObject(cropRecommendationResult)),
                    ChatMessage.FromSystem(JsonConvert.SerializeObject(cropPricingModel)),
                    ChatMessage.FromUser(
                        string.Format(Constants.CropRecommendationUserPrompt, 
                            cropRecommendationRequest.LandSizeInSqureMeters, 
                            cropRecommendationRequest.Location, 
                            cropRecommendationRequest.Budget, 
                            cropRecommendationRequest.StartDate)),
                }
            });
        
        return completionResult.Successful ? 
            JsonConvert.DeserializeObject<CropRecommendationResponse>(completionResult.Choices.First().Message.Content!) : 
            new CropRecommendationResponse();
    }
}
