namespace FarmMate.Farming.API.Models;

public class ProcessDailyTaskRequest
{
    public string CropName { get; set; }
    
    public string Location { get; set; }
    
    public int HarvestInDays { get; set; }
    
    public double LandSizeInSqureMeters { get; set; }
    
    public double Budget { get; set; }
    
    public DateTime StartDate { get; set; }
}
