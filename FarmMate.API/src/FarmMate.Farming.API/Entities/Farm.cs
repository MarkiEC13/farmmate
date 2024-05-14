using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using FarmMate.Common.Validators;

namespace FarmMate.Farming.API.Entities;

[ExcludeFromCodeCoverage]
public class Farm
{
    [Required]
    [NotEmptyGuid]
    public Guid Id { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    [Required]
    public string Location { get; set; }
    
    [Required]
    public double Size { get; set; }
    
    public string? Coordinates { get; set; }
    
    [Required]
    public double Budget { get; set; }
    
    [Required]
    public DateTime StartDateTime { get; set; }
    
    [Required]
    public string Crop { get; set; }
    
    [Required]
    public double EstimatedCost { get; set; }
    
    [Required]
    public double EstimatedRevenue { get; set; }
    
    [Required]
    public int HarvestInDays { get; set; }
    
    [Required]
    public DateTime CreatedDateTime { get; set; }

    [Required]
    public DateTime UpdatedDateTime { get; set; }
}
