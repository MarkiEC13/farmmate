using Azure.Core.Serialization;
using FarmMate.Common.Exceptions;
using FarmMate.Farming.API.Entities;
using FarmMate.Farming.API.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using TestProject1.Fakers;
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
    
    [Fact]
    public async Task CreateFarm_ReturnsFarml_WhenFarmIsValid()
    {
        // Arrange
        var farm = new FakerFarm().Generate(1).First();
        var json = JsonConvert.SerializeObject(farm);
        var request = FakeHttpRequestDataExtensions.GetRequestData(json, "post", null, _functionContext);

        // Act
        var result = await FarmHttpTrigger.CreateFarm(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(farm.Crop, result.Crop);
        Assert.Equal(farm.Location, result.Location);
        Assert.Equal(farm.Budget, result.Budget);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.True((DateTime.UtcNow - result.CreatedDateTime).TotalSeconds < 5);
        Assert.True((DateTime.UtcNow - result.UpdatedDateTime).TotalSeconds < 5);
    }
    
    [Fact]
    public async Task CreateFarm_ThrowsBadRequestException_WhenFarmIsInvalid()
    {
        // Arrange
        var request = FakeHttpRequestDataExtensions.GetRequestData("{}", "post", null, _functionContext);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => FarmHttpTrigger.CreateFarm(request));
    }
    
    [Fact]
    public async Task UpdateFarm_ReturnsUpdatedFarm_WhenFarmIsValid()
    {
        // Arrange
        var existingFarm = new FakerFarm().Generate(1).First();
        var farm = new List<Farm> { existingFarm }.AsReadOnly();

        var updatedFarm = new FakerFarm(existingFarm.Id).Generate(1).First();
        var json = JsonConvert.SerializeObject(updatedFarm);
        var request = FakeHttpRequestDataExtensions.GetRequestData(json, "post", null, _functionContext);

        // Act
        var result = await FarmHttpTrigger.UpdateFarm(request, existingFarm.Id, farm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedFarm.Crop, result.Crop);
        Assert.Equal(updatedFarm.Location, result.Location);
        Assert.Equal(updatedFarm.Budget, result.Budget);
        Assert.True((DateTime.UtcNow - result.UpdatedDateTime).TotalSeconds < 5);
    }
    
    [Fact]
    public async Task UpdateFarm_ThrowsBadRequestException_WhenFarmIsInvalid()
    {
        // Arrange
        var request = FakeHttpRequestDataExtensions.GetRequestData("{}", "post", null, _functionContext);

        var existingFarm = new FakerFarm().Generate(1).First();
        var farms = new List<Farm> { existingFarm }.AsReadOnly();
        var id = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => FarmHttpTrigger.UpdateFarm(request, id, farms));
    }
    
    [Fact]
    public async Task UpdateFarm_ThrowsNotFoundException_WhenFarmDoesNotExist()
    {
        // Arrange
        var farm = new FakerFarm().Generate(1).First();
        var json = JsonConvert.SerializeObject(farm);
        var request = FakeHttpRequestDataExtensions.GetRequestData(json, "post", null, _functionContext);

        var farms = new List<Farm>().AsReadOnly();
        var id = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => FarmHttpTrigger.UpdateFarm(request, id, farms));
    }
    
    [Fact]
    public void GetFarms_ReturnsFarm_WhenFarmsExist()
    {
        // Arrange
        var farms = new FakerFarm().Generate(2);

        // Act
        var result = FarmHttpTrigger.GetFarmsByUserId(_request, farms.AsReadOnly());

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(farms, result);
    }
    
    [Fact]
    public void GetFarms_ThrowsNotFoundException_WhenNoFarmsExist()
    {
        // Arrange
        var users = new List<Farm>().AsReadOnly();

        // Act & Assert
        Assert.Throws<NotFoundException>(() => FarmHttpTrigger.GetFarmById(_request, users));
    }
    
    [Fact]
    public void GetFarmById_ReturnsFarm_WhenFarmExists()
    {
        // Arrange
        var farm = new FakerFarm().Generate(1).First();
        var farms = new List<Farm> { farm }.AsReadOnly();

        // Act
        var result = FarmHttpTrigger.GetFarmById(_request, farms);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(farm, result);
    }

    [Fact]
    public void GetFarmById_ThrowsNotFoundException_WhenFarmDoesNotExist()
    {
        // Arrange
        var farm = new List<Farm>().AsReadOnly();

        // Act & Assert
        Assert.Throws<NotFoundException>(() => FarmHttpTrigger.GetFarmById(_request, farm));
    }
}
