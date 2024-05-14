using System.Diagnostics.CodeAnalysis;
using System.Net;
using FarmMate.Common.Exceptions;
using FarmMate.Common.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace FarmMate.Common.Middlewares;

[ExcludeFromCodeCoverage]
public class GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    : IFunctionsWorkerMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException e)
        {
            await HandleResponse(context, HttpStatusCode.NotFound, e);
        }
        catch (BadRequestException e)
        {
            await HandleResponse(context, HttpStatusCode.BadRequest, e);
        }
    }

    private async Task HandleResponse(FunctionContext context, HttpStatusCode statusCode, Exception e)
    {
        _logger.LogError(e, "Error processing invocation");

        var requestData = await context.GetHttpRequestDataAsync();
        if (requestData != null)
        {
            var responseData = await requestData.CreateResponseAsync<string>(statusCode);

            var invocationResult = context.GetInvocationResult();
            invocationResult.Value = responseData;
        }
    }
}
