using System.Text.Json;
using FarmMate.Common.Middlewares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenAI.Extensions;
using OpenAI.ObjectModels;

var host = new HostBuilder()
    .ConfigureAppConfiguration(configBuilder =>
        configBuilder
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables())
    .ConfigureFunctionsWorkerDefaults(
        builder =>
        {
            builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    )
    .ConfigureServices(services =>
    {
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        services.AddSingleton<IOpenApiConfigurationOptions>(_ => new OpenApiConfigurationOptions
        {
            Info = new OpenApiInfo
            {
                Version = "1.0.0",
                Title = "FarmMate Crop Recommendation Service",
                Description = "This provides access to the FarmMate rop Recommendation Service."
            },
            Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
            OpenApiVersion = OpenApiVersionType.V3,
            ForceHttps = false,
            ForceHttp = false,
        });

        services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();

        _ = services.AddOpenAIService(settings =>
        {
            settings.ApiKey = Environment.GetEnvironmentVariable("OpenAIServiceOptions__ApiKey") ?? throw new ArgumentNullException("OpenAI API key required");
            settings.DefaultModelId = Models.Gpt_4_turbo_preview;
        });
    })
    .Build();

await host.RunAsync();
