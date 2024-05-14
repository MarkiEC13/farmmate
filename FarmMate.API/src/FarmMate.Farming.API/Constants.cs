namespace FarmMate.Farming.API;

public class Constants
{
    public const string FarmsTableName = "[farm].[Farms]";
    public const string FarmDailyTableName = "[farm].[FarmDaily]";
    public const string FarmDailyTaskTableName = "[farm].[FarmDailyTask]";
    
    public const string ConnectionStringSetting = "FarmContext";
    
    public const string DailyTasksSystemRolePrompt = "Assume you are an expert in the agriculture sector, specifically in the context of Sri Lanka.";
    public const string DailyTasksConversationTypePrompt = "You are having a conversation with a farmer who is planning to cultivate crops. You must guide the farmer through the farming process. These farmers are new to the agriculture sector, so be specific about what must be completed daily. You must give clear instructions or tasks each day.  Each day should have at least 4 days and it is okay to repeat tasks in close days.";
    public const string DailyTasksResponseFormatPrompt = @"You must respond only in the following JSON format:
                        {
                          'tasks': [
                             {
                                   'day': 1,
                                  'todos': [
                                       'Install irrigation system for regular watering needs.',
                                       'Till the soil to a fine tilth to ensure proper seedbed preparation.',
                                       'Apply base fertilizer as per soil test recommendations.',
                                       'Form planting beds that are 15 cm high and 75 cm wide.'.
                                  ]
                             },
                             {
                                     'day': 2,
                                    'todos': [
                                             'Water the seeds gently to ensure good seed-soil contact.',
                                            'Cover the seeds with a fine layer of compost or peat moss.',
                                           'Sow leek seeds shallowly, approximately 1 cm deep.',
                                          'Space seeds 6 cm apart in rows.'
                                    ]
                             }
                            ]
                        }";
    public const string DailyTasksUserPrompt = "I'm going to cultivate carrots at {0} Farm. This takes {1} days to get the harvest. Please specify the tasks I need to complete from day {2} to day {3}.";
}
