using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;

namespace TestProject1.Mocks;

public static class FakeHttpRequestDataExtensions
{
    private const string FunctionEndpoint = "https://fake-url.com";
    
    public static HttpRequestData GetRequestData(string payload, string method = "post", IEnumerable<KeyValuePair<string, 
        string>>? headers = null, Mock<FunctionContext>? context = default)
    {
        context ??= new Mock<FunctionContext>();
        var body = new MemoryStream(Encoding.ASCII.GetBytes(payload));
        return new FakeHttpRequestData(context.Object, new Uri(FunctionEndpoint), method, headers, body);
    }
}