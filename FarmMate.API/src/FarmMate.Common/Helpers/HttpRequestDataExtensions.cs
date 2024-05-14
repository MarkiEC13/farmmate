using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.Azure.Functions.Worker.Http;

namespace FarmMate.Common.Helpers;

[ExcludeFromCodeCoverage]
public static class HttpRequestDataExtensions
{
    public static async Task<HttpResponseData> CreateResponseAsync<T>(this HttpRequestData req,
        HttpStatusCode statusCode, T? model = null) where T : class
    {
        var response = req.CreateResponse(statusCode);

        if (model != null) await response.WriteAsJsonAsync(model);
        return response;
    }
}
