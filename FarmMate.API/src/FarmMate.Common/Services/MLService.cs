using System.Text;
using Newtonsoft.Json;

namespace FarmMate.Common.Services;

public class MLService(HttpClient httpClient) : IMLService
{
    private const string MLEndpoint = "https://farmmate,{0}.westeurope.inference.ml.azure.com";
    
    public async Task<object> GetPredictionAsync(object request, string model)
    {
        var jsonContent = JsonConvert.SerializeObject(request);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var endpoint = string.Format(MLEndpoint, model);
        HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

        if (!response.IsSuccessStatusCode) return null;
        
        var responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
}

public interface IMLService
{
    public Task<object> GetPredictionAsync(object request, string model);
}