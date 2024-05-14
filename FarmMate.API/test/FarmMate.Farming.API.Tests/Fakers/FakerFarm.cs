using Bogus;
using FarmMate.Farming.API.Entities;

namespace TestProject1.Fakers;

public sealed class FakerFarm : Faker<Farm>
{
    public FakerFarm(Guid id) {
        RuleFor(o => o.Id, id);
        RuleFor(o => o.UserId, f => f.Random.Guid().ToString());
        RuleFor(o => o.Location, f => f.Address.City());
        RuleFor(o => o.Size, f => f.Random.Double(1, 100));
        RuleFor(o => o.Coordinates, f => f.Address.Latitude() + ", " + f.Address.Longitude());
        RuleFor(o => o.Budget, f => f.Random.Double(1, 100));
        RuleFor(o => o.StartDateTime, f => f.Date.Future());
        RuleFor(o => o.Crop, f => f.Commerce.ProductName());
        RuleFor(o => o.EstimatedCost, f => f.Random.Double(1, 100));
        RuleFor(o => o.EstimatedRevenue, f => f.Random.Double(1, 100));
        RuleFor(o => o.HarvestInDays, f => f.Random.Int(1, 100));
        RuleFor(o => o.UpdatedDateTime, f => f.Date.Future());
        RuleFor(o => o.CreatedDateTime, f => f.Date.Recent());
    }
    
    public FakerFarm() : this(Guid.NewGuid()) { }
}