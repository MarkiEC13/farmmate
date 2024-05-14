using System.ComponentModel.DataAnnotations;
using FarmMate.Common.Validators;

namespace FarmMate.Farming.API.Entities;

public class FarmDailyTask
{
    [Required]
    [NotEmptyGuid]
    public Guid Id { get; set; }
    
    [Required]
    [NotEmptyGuid]
    public Guid FarmDailyId { get; set; }
    
    [Required]
    [NotEmptyGuid]
    public Guid FarmId { get; set; }
    
    [Required]
    public int Day { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    [Required]
    public bool IsCompleted { get; set; }
    
    [Required]
    public DateTime CreatedDateTime { get; set; }

    [Required]
    public DateTime UpdatedDateTime { get; set; }
}
