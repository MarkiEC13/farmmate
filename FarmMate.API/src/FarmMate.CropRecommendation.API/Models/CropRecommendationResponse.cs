namespace FarmMate.CropRecommendation.API.Models;

public class CropRecommendationResponse
{
    public CropRecommendation[] Crops { get; set; }
}

public class CropRecommendation
{
    public string Name { get; set; }
    
    public string SeedsRequired { get; set; }
    
    public double EstimatedCost { get; set; }
    
    public double EstimatedRevenue { get; set; }
    
    public int HarvestInDays { get; set; }
}
