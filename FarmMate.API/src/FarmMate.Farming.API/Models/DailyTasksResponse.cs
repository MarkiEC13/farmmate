namespace FarmMate.Farming.API.Models;

public class DailyTasksResponse
{
    public DailyItem[] Tasks { get; set; }
}

public class DailyItem
{
    public int Day { get; set; }

    public string[] Todos { get; set; }
}
