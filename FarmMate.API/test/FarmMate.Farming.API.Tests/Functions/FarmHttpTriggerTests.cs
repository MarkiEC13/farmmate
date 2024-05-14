using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using TestProject1.Mocks;

namespace TestProject1.Functions;

public class FarmHttpTriggerTests
{
    private readonly Mock<FunctionContext> _functionContext;
    private readonly HttpRequestData _request;

    public FarmHttpTriggerTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(Options.Create(new WorkerOptions{Serializer = new JsonObjectSerializer()}));

        var serviceProvider = serviceCollection.BuildServiceProvider();
        _functionContext = new Mock<FunctionContext>();
        _functionContext.Setup(i => i.InstanceServices)
            .Returns(serviceProvider);
        
        _request = FakeHttpRequestDataExtensions.GetRequestData(String.Empty);
    }
}
