using System.Collections.Generic;

public static class NpcActions
{
    public const string HUNGER_BREAD = "HUNGER_BREAD";
    public const string HUNGER_MEAT = "HUNGER_MEAT";

    public const string THIRST = "THIRST";

    public const string SLEEP = "SLEEP";
    public const string RESTING = "RESTING";

    public const string EDUCATION_SCHOOL = "EDUCATION_SCHOOL";
    public const string EDUCATION_LIBRARY = "EDUCATION_LIBRARY";
    public const string EDUCATION_COLLEGE = "EDUCATION_COLLEGE";

    public const string COMMUNICATION_CHAT = "COMMUNICATION_CHAT";

    public const string RELIGION_CHURCH = "RELIGION_CHURCH";

    public const string RECREATION_SOCCER = "RECREATION_SOCCER";
    public const string RECREATION_TAVERN = "RECREATION_TAVERN";
    public const string RECREATION_VISIT = "RECREATION_VISIT";
    public const string RECREATION_THEATER = "RECREATION_THEATER";
    public const string RECREATION_CAFE = "RECREATION_CAFE";
    public const string RECREATION_SHOP = "RECREATION_SHOP";
    public const string RECREATION_SHOOTING = "RECREATION_SHOOTING";
    public const string RECREATION_BARBECUE = "RECREATION_BARBECUE";

    public const string WORK_HUNTING = "WORK_HUNTING";
    public const string WORK_BUTCHER = "WORK_BUTCHER";
    public const string WORK_GRAIN_FARMING = "WORK_GRAIN_FARMING";
    public const string WORK_WINDMILL = "WORK_WINDMILL";
    public const string WORK_BAKERY = "WORK_BAKERY";
    public const string WORK_HOPS_FARMING = "WORK_HOPS_FARMING";
    public const string WORK_BREWERY = "WORK_BREWERY";
    public const string WORK_LOGGING = "WORK_LOGGING";
    public const string WORK_CARPENTER = "WORK_CARPENTER";
    public const string WORK_MINING = "WORK_MINING";
    public const string WORK_BLACKSMITH = "WORK_BLACKSMITH";

    public static string[] ALL_ACTIONS = new string[] {
        HUNGER_BREAD, HUNGER_MEAT,
        THIRST,
        SLEEP,
        EDUCATION_SCHOOL, EDUCATION_LIBRARY, EDUCATION_COLLEGE,
        COMMUNICATION_CHAT,
        RELIGION_CHURCH,
        RECREATION_SOCCER, RECREATION_TAVERN, RESTING, RECREATION_VISIT, RECREATION_CAFE,
        RECREATION_THEATER, RECREATION_SHOP, RECREATION_SHOOTING, RECREATION_BARBECUE,
        WORK_HUNTING, WORK_BUTCHER, WORK_GRAIN_FARMING, WORK_WINDMILL, WORK_BAKERY, WORK_HOPS_FARMING,
        WORK_BREWERY, WORK_LOGGING, WORK_CARPENTER , WORK_MINING, WORK_BLACKSMITH
    };

    public static string[] WORK_ACTIONS = new string[]
    {
        WORK_HUNTING, WORK_BUTCHER, WORK_GRAIN_FARMING, WORK_WINDMILL, WORK_BAKERY, WORK_HOPS_FARMING,
        WORK_BREWERY, WORK_LOGGING, WORK_CARPENTER, WORK_MINING, WORK_BLACKSMITH
    };

    public static string[] RECREATION_ACTIONS = new string[]
{
        RECREATION_SOCCER, RECREATION_TAVERN, RECREATION_VISIT, RECREATION_THEATER,
        RECREATION_CAFE, RECREATION_SHOP, RECREATION_SHOOTING, RECREATION_BARBECUE
};

    private static Dictionary<int, string> _actionIdToNameMapping;
    private static Dictionary<string, int> _actionNameToIdMapping;

    public static string actionNameFromId(int actionId)
    {
        if (_actionIdToNameMapping == null)
        {
            _setupMapping();
        }
        return _actionIdToNameMapping[actionId];
    }

    public static int actionIdFromName(string actionName)
    {
        if (_actionNameToIdMapping == null)
        {
            _setupMapping();
        }
        return _actionNameToIdMapping[actionName];
    }

    private static void _setupMapping()
    {
        _actionIdToNameMapping = new Dictionary<int, string>();
        _actionNameToIdMapping = new Dictionary<string, int>();

        _actionIdToNameMapping[0] = HUNGER_BREAD;
        _actionIdToNameMapping[1] = HUNGER_MEAT;
        _actionIdToNameMapping[2] = THIRST;
        _actionIdToNameMapping[3] = SLEEP;
        _actionIdToNameMapping[4] = EDUCATION_SCHOOL;
        _actionIdToNameMapping[5] = EDUCATION_LIBRARY;
        _actionIdToNameMapping[6] = EDUCATION_COLLEGE;
        _actionIdToNameMapping[7] = COMMUNICATION_CHAT;
        _actionIdToNameMapping[8] = RELIGION_CHURCH;
        _actionIdToNameMapping[9] = RECREATION_SOCCER;
        _actionIdToNameMapping[10] = RECREATION_TAVERN;
        _actionIdToNameMapping[11] = RECREATION_CAFE;
        _actionIdToNameMapping[12] = RECREATION_THEATER;
        _actionIdToNameMapping[13] = RECREATION_SHOP;
        _actionIdToNameMapping[14] = RECREATION_SHOOTING;
        _actionIdToNameMapping[15] = RECREATION_BARBECUE;
        _actionIdToNameMapping[16] = RESTING;
        _actionIdToNameMapping[17] = RECREATION_VISIT;
        _actionIdToNameMapping[18] = WORK_HUNTING;
        _actionIdToNameMapping[19] = WORK_BUTCHER;
        _actionIdToNameMapping[20] = WORK_GRAIN_FARMING;
        _actionIdToNameMapping[21] = WORK_WINDMILL;
        _actionIdToNameMapping[22] = WORK_BAKERY;
        _actionIdToNameMapping[23] = WORK_HOPS_FARMING;
        _actionIdToNameMapping[24] = WORK_BREWERY;
        _actionIdToNameMapping[25] = WORK_LOGGING;
        _actionIdToNameMapping[26] = WORK_CARPENTER;
        _actionIdToNameMapping[27] = WORK_MINING;
        _actionIdToNameMapping[28] = WORK_BLACKSMITH;
    
        foreach (int actionId in _actionIdToNameMapping.Keys)
        {
            string actionName = _actionIdToNameMapping[actionId];
            _actionNameToIdMapping[actionName] = actionId;
        }
    }
}
