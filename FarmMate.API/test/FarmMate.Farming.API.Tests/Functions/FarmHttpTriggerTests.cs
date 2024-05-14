using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;

namespace TestProject1.Functions;

public class FarmHttpTriggerTests
{
    private readonly Mock<FunctionContext> _functionContext;
    private readonly HttpRequestData _request;

}
