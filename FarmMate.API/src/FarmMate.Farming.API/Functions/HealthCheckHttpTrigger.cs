using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;

namespace FarmMate.Farming.API.Functions;

public class HealthCheckHttpTrigger
{
    [OpenApiOperation(operationId: nameof(HealthCheck), tags: ["Health API"], Summary = "Health Check", Description = "Returns OK if things are working", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Summary = "Health Check", Description = "This returns OK if things are working")]
    [Function("HealthCheck")]
    public HttpResponseData HealthCheck([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req)
    {
        return req.CreateResponse(HttpStatusCode.OK);
    }
}
