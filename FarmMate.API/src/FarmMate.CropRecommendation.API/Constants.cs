namespace FarmMate.CropRecommendation.API;

public static class Constants
{
    public const string SystemRolePrompt = "Assume you are an expert in the agriculture sector, specifically in the context of Sri Lanka.";
    
    public const string ConversationTypePrompt =
        "You are having a conversation with a farmer who is planning to cultivate in the next season. You must recommend 5 crops.";

    public const string CropRecommendationResponseFormatPrompt = @"You must respond only in the following JSON format:
                        {
                          'crops': [
                            {
                              'name': 'example crop',
                              'seedsRequired': 'example amount in KG without units',
                              'estimatedCost': 'numeric value only',
                              'estimatedRevenue': 'numeric value only',
                              'harvestInDays': 'numeric days without any text or range'
                            }
                          ]
                        }
                        Do not include any units, symbols, or ranges. Use plain integers for monetary values and days except seed required.";
    
    public const string CropRecommendationUserPrompt = "I have a farm with {0} land size in square meters in {1} and my budget is around {2} and planning to start on {3}. Can you recommend some crops for a farmer who is planning to cultivate in the next season?";
}