using UnityEngine;
using System.Collections.Generic;

public class ResourceHandler
{
    public const string WOODEN_LOG = "LOG";
    public const string FIREWOOD = "FIREWOOD";

    public const string RAW_VENISON = "RAW_VENISON";
    public const string COOKED_MEAT = "COOKED_MEAT";

    public const string GRAIN = "GRAIN";
    public const string FLOUR = "FLOUR";
    public const string BREAD = "BREAD";

    public const string HOPS = "HOPS";
    public const string BEER = "BEER";

    public const string ORE = "ORE";
    public const string TOOLS = "TOOLS";

    public static string[] ALL_RESOURCES = new string[] {
        WOODEN_LOG, FIREWOOD,
        RAW_VENISON, COOKED_MEAT,
        GRAIN, FLOUR, BREAD,
        HOPS, BEER,
        ORE, TOOLS
    };

    public static string[] PRODUCTION_CHAIN_BREAD_RESOURCES = new string[] {
        GRAIN, FLOUR, BREAD
    };
    public static string[] PRODUCTION_CHAIN_HOPS_RESOURCES = new string[] {
        HOPS, BEER
    };
    public static string[] PRODUCTION_CHAIN_MEAT_RESOURCES = new string[] {
        RAW_VENISON, COOKED_MEAT
    };

    private const float DOUBLE_CONSUME_THRESHOLD = 0.7f;

    private Dictionary<string, int> _resources;
    private Dictionary<string, int> _capacities;

    public ResourceHandler()
    {
        reset();
    }

    public void addResource(int amount, string resourceId)
    {
        _resources[resourceId] = Mathf.Clamp(_resources[resourceId] + amount, 0, _capacities[resourceId]);
    }

    public void useResource(int useAmount, string resourceId)
    {
        if (GameAcademy.IGNORE_ALL_RESOURCES) return;

        int resourceAmount = _resources[resourceId];
        if (getResourceNormalized(resourceId) > DOUBLE_CONSUME_THRESHOLD) useAmount *= 2;
        int newResourceAmount = resourceAmount - useAmount;
        if (newResourceAmount < 0)
        {
            Debug.LogWarning("Used more resources then available! Used " + useAmount.ToString() + " " + resourceId + ".");
        }
        _resources[resourceId] = Mathf.Clamp(newResourceAmount, 0, _capacities[resourceId]);
    }

    public int getResource(string resourceId)
    {
        return _resources[resourceId];
    }

    public int getResourceCapacity(string resourceId)
    {
        return _capacities[resourceId];
    }

    public float getResourceNormalized(string resourceId)
    {
        return (float)_resources[resourceId] / (float)_capacities[resourceId];
    }

    public void reset()
    {
        int numAgents = GameAcademy.NUM_AGENTS_PER_VILLAGE;
        _resources = new Dictionary<string, int>();
        _capacities = new Dictionary<string, int>();

        _capacities[WOODEN_LOG] = numAgents * 10;
        _capacities[FIREWOOD] = numAgents * 10;
        _capacities[RAW_VENISON] = numAgents * 10;
        _capacities[COOKED_MEAT] = numAgents * 10;
        _capacities[GRAIN] = numAgents * 10;
        _capacities[FLOUR] = numAgents * 10;
        _capacities[BREAD] = numAgents * 10;
        _capacities[HOPS] = numAgents * 10;
        _capacities[BEER] = numAgents * 10;
        _capacities[ORE] = numAgents * 10;
        _capacities[TOOLS] = numAgents * 10;

        _resources[WOODEN_LOG] = Mathf.RoundToInt(((float)_capacities[WOODEN_LOG] / 2f) * (0.5f + Random.value));
        _resources[FIREWOOD] = Mathf.RoundToInt(((float)_capacities[FIREWOOD] / 2f) * (0.5f + Random.value));
        _resources[RAW_VENISON] = Mathf.RoundToInt(((float)_capacities[RAW_VENISON] / 2f) * (0.5f + Random.value));
        _resources[COOKED_MEAT] = Mathf.RoundToInt(((float)_capacities[COOKED_MEAT] / 2f) * (0.5f + Random.value));
        _resources[GRAIN] = Mathf.RoundToInt(((float)_capacities[GRAIN] / 2f) * (0.5f + Random.value));
        _resources[FLOUR] = Mathf.RoundToInt(((float)_capacities[FLOUR] / 2f) * (0.5f + Random.value));
        _resources[BREAD] = Mathf.RoundToInt(((float)_capacities[BREAD] / 2f) * (0.5f + Random.value));
        _resources[HOPS] = Mathf.RoundToInt(((float)_capacities[HOPS] / 2f) * (0.5f + Random.value));
        _resources[BEER] = Mathf.RoundToInt(((float)_capacities[BEER] / 2f) * (0.5f + Random.value));
        _resources[ORE] = Mathf.RoundToInt(((float)_capacities[ORE] / 2f) * (0.5f + Random.value));
        _resources[TOOLS] = Mathf.RoundToInt(((float)_capacities[TOOLS] / 2f) * (0.5f + Random.value));
    }
}
