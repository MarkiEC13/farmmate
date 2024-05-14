using System.ComponentModel.DataAnnotations;

namespace FarmMate.CropRecommendation.API.Models;

public class CropRecommendationRequest
{
    [Required]
    public string Location { get; set; }
    
    [Required]
    public double LandSizeInSqureMeters { get; set; }
    
    [Required]
    public double Budget { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
}
