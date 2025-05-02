using UnityEngine;
using System.Collections.Generic;

public static class AttributeHandler
{
    private static Dictionary<string, string[]> _bonusActions;
    private static Dictionary<string, string[]> _malusActions;

    private static void _init() {
        _bonusActions = new Dictionary<string, string[]>();
        _bonusActions[NpcState.ATT_COMMODITIES] = new string[] { NpcActions.RECREATION_SHOP };
        _bonusActions[NpcState.ATT_INTELLIGENCE] = new string[] { NpcActions.RECREATION_THEATER };
        _bonusActions[NpcState.ATT_COMPANIONSHIP] = new string[] { NpcActions.RECREATION_VISIT, NpcActions.RECREATION_BARBECUE, NpcActions.RECREATION_CAFE };
        _bonusActions[NpcState.ATT_FEROCITY] = new string[] { NpcActions.RECREATION_SOCCER, NpcActions.RECREATION_SHOOTING };
        _bonusActions[NpcState.ATT_FITNESS] = new string[] { NpcActions.RECREATION_SOCCER };

        _malusActions = new Dictionary<string, string[]>();
        _malusActions[NpcState.ATT_COMMODITIES] = new string[] { NpcActions.RECREATION_VISIT };
        _malusActions[NpcState.ATT_INTELLIGENCE] = new string[] { NpcActions.RECREATION_TAVERN };
        _malusActions[NpcState.ATT_COMPANIONSHIP] = new string[] { NpcActions.RECREATION_SHOOTING };
        _malusActions[NpcState.ATT_FEROCITY] = new string[] { NpcActions.RECREATION_CAFE, NpcActions.RECREATION_SHOP };
        _malusActions[NpcState.ATT_FITNESS] = new string[] { NpcActions.RECREATION_BARBECUE, NpcActions.RECREATION_TAVERN };
    }

    public static Dictionary<string, float> getAttributesByJobId(string jobId)
    {
        if (_bonusActions == null || _malusActions == null) _init();

        Dictionary<string, float> attributes = new Dictionary<string, float>();
        attributes[NpcState.ATT_COMMODITIES] = Random.value;
        attributes[NpcState.ATT_INTELLIGENCE] = Random.value;
        attributes[NpcState.ATT_COMPANIONSHIP] = Random.value;
        attributes[NpcState.ATT_FITNESS] = Random.value;
        attributes[NpcState.ATT_FEROCITY] = Random.value;

        float defaultAdd = 0.25f;
        switch (jobId)
        {
            case JobHandler.JOB_BAKER:
                attributes[NpcState.ATT_COMMODITIES] += defaultAdd;
                attributes[NpcState.ATT_FITNESS] -= defaultAdd;
                attributes[NpcState.ATT_FEROCITY] -= defaultAdd;
                attributes[NpcState.ATT_COMPANIONSHIP] -= defaultAdd;
                attributes[NpcState.ATT_INTELLIGENCE] += defaultAdd;
                break;

            case JobHandler.JOB_BLACKSMITH:
                attributes[NpcState.ATT_COMMODITIES] += defaultAdd;
                attributes[NpcState.ATT_FEROCITY] += defaultAdd;
                attributes[NpcState.ATT_COMPANIONSHIP] += defaultAdd;
                break;

            case JobHandler.JOB_BREWER:
                attributes[NpcState.ATT_FEROCITY] -= defaultAdd;
                attributes[NpcState.ATT_INTELLIGENCE] += defaultAdd;
                break;

            case JobHandler.JOB_BUTCHER:
                attributes[NpcState.ATT_COMMODITIES] += defaultAdd;
                attributes[NpcState.ATT_COMPANIONSHIP] -= defaultAdd;
                attributes[NpcState.ATT_INTELLIGENCE] -= defaultAdd;
                break;

            case JobHandler.JOB_CARPENTER:
                attributes[NpcState.ATT_COMMODITIES] += defaultAdd;
                attributes[NpcState.ATT_FITNESS] -= defaultAdd;
                attributes[NpcState.ATT_INTELLIGENCE] += defaultAdd;
                break;

            case JobHandler.JOB_MILLER:
                attributes[NpcState.ATT_FEROCITY] -= defaultAdd;
                break;

            case JobHandler.JOB_GRAIN_FARMER:
                attributes[NpcState.ATT_COMMODITIES] -= defaultAdd;
                attributes[NpcState.ATT_COMPANIONSHIP] += defaultAdd;
                attributes[NpcState.ATT_INTELLIGENCE] -= defaultAdd;
                break;

            case JobHandler.JOB_HOPS_FARMER:
                attributes[NpcState.ATT_COMMODITIES] -= defaultAdd;
                attributes[NpcState.ATT_COMPANIONSHIP] += defaultAdd;
                attributes[NpcState.ATT_INTELLIGENCE] -= defaultAdd;
                break;

            case JobHandler.JOB_HUNTER:
                attributes[NpcState.ATT_FITNESS] += defaultAdd;
                attributes[NpcState.ATT_FEROCITY] += defaultAdd;
                attributes[NpcState.ATT_COMPANIONSHIP] -= defaultAdd;
                break;

            case JobHandler.JOB_LOGGER:
                attributes[NpcState.ATT_FITNESS] += defaultAdd;
                attributes[NpcState.ATT_FEROCITY] += defaultAdd;
                attributes[NpcState.ATT_INTELLIGENCE] -= defaultAdd;
                break;

            case JobHandler.JOB_MINER:
                attributes[NpcState.ATT_FITNESS] += defaultAdd;
                attributes[NpcState.ATT_COMMODITIES] -= defaultAdd;
                attributes[NpcState.ATT_FEROCITY] += defaultAdd;
                attributes[NpcState.ATT_COMPANIONSHIP] += defaultAdd;
                attributes[NpcState.ATT_INTELLIGENCE] -= defaultAdd;
                break;
        }

        Dictionary<string, float> clampedAttributes = new Dictionary<string, float>();
        foreach (string attributeId in attributes.Keys) clampedAttributes[attributeId] = Mathf.Clamp01(attributes[attributeId]);
        return clampedAttributes;
    }

    public static string getPreferredAttributeAction(string attribute, List<string> mask)
    {
        List<string> malusActions = new List<string>(_malusActions[attribute]);
        List<string> possibleBonusActions = new List<string>();

        foreach (string actionId in _bonusActions[attribute])
        {
            if (mask.Contains(actionId) || malusActions.Contains(actionId)) continue;
            else possibleBonusActions.Add(actionId);
        }
        if (possibleBonusActions.Count == 0) return null;
        return possibleBonusActions[Mathf.RoundToInt(Random.value * (possibleBonusActions.Count - 1))];
    }
}            