using UnityEngine;
using System.Collections.Generic;

public class ManualDecisionMaker
{
    private NpcState _npcState;
    private NpcAgent _npc;
    private List<string> _mask;

    public ManualDecisionMaker(NpcAgent npc)
    {
        _npc = npc;
        _npcState = npc.NpcState;
    }

    public string makeDecision()
    {
        _mask = _npc.ActionMaskingHandler.getStringActionMask(_npc);

        if (_shouldSleep()) return NpcActions.SLEEP;
        if (_shouldWork()) return _npc.JobHandler.getActionIdFromJobId(_npc.NpcState.AgentInfo.JobId);
        if (_shouldRest()) return NpcActions.RESTING;

        string action = _makeRecreationDecision();
        if (action != null) return action;

        action = _makeNeedDecision();
        if (action != null) return action;

        Debug.LogWarning("Could not find a good action for the agent. Choosing a random action that is not masked instead.");
        List<string> unmaskedActions = new List<string>();
        foreach (string actionId in NpcActions.ALL_ACTIONS)
        {
            if (!_mask.Contains(actionId)) unmaskedActions.Add(actionId);
        }
        if (unmaskedActions.Count != 0) {
            return unmaskedActions[Mathf.RoundToInt(Random.value * (unmaskedActions.Count - 1))];
        }
        Debug.LogWarning("Could not manage to find ANY actions for the agent! Choosing an action that IS MASKED instead.");
        return NpcActions.ALL_ACTIONS[Mathf.RoundToInt(Random.value * (NpcActions.ALL_ACTIONS.Length - 1))];
    }

    private bool _isMasked(string actionId)
    {
        return _mask.Contains(actionId);
    }

    private bool _shouldSleep()
    {
        return _npcState.Sleep < 0.15f && !_isMasked(NpcActions.SLEEP);
    }

    private bool _shouldWork()
    {
        return _npcState.Work < 0.15f && !_isMasked(_npc.NpcState.AgentInfo.JobId);
    }

    private bool _shouldRest()
    {
        return _npcState.Exhaustion <= 0.35f && !_isMasked(NpcActions.RESTING);
    }

    private string _makeNeedDecision()
    {
        List<KeyValuePair<string, float>> needs = new List<KeyValuePair<string, float>>();
        foreach (string needType in NpcState.ALL_NEEDS)
        {
            needs.Add(new KeyValuePair<string, float>(needType, _npcState.getNeedByType(needType)));
        }
        needs.Sort((x, y) => x.Value.CompareTo(y.Value));

        foreach (KeyValuePair<string, float> need in needs)
        {
            string decision = null;
            switch (need.Key)
            {
                case NpcState.NEED_HUNGER:
                    decision = _makeHungerDecision();
                    break;
                case NpcState.NEED_THIRST:
                    decision = _makeThirstDecision();
                    break;
                case NpcState.NEED_EDUCATION:
                    decision = _makeEducationDecision();
                    break;
                case NpcState.NEED_COMMUNICATION:
                    decision = _makeCommunicationDecision();
                    break;
                case NpcState.NEED_FAITH:
                    decision = _makeFaithDecision();
                    break;
            }
            if (decision == null) continue;
            else return decision;
        }
        return null;
    }

    private string _makeHungerDecision()
    {
        string firstAction = Random.value > 0.5f ? NpcActions.HUNGER_BREAD : NpcActions.HUNGER_MEAT;
        string secondAction = firstAction == NpcActions.HUNGER_BREAD ? NpcActions.HUNGER_MEAT : NpcActions.HUNGER_BREAD;
        if (!_isMasked(firstAction)) return firstAction;
        if (!_isMasked(secondAction)) return secondAction;
        return null;
    }

    private string _makeEducationDecision()
    {
        if (!_isMasked(NpcActions.EDUCATION_SCHOOL)) return NpcActions.EDUCATION_SCHOOL;
        if (!_isMasked(NpcActions.EDUCATION_COLLEGE)) return NpcActions.EDUCATION_COLLEGE;
        if (!_isMasked(NpcActions.EDUCATION_LIBRARY)) return NpcActions.EDUCATION_LIBRARY;
        return null;
    }

    private string _makeThirstDecision()
    {
        return _isMasked(NpcActions.THIRST) ? null : NpcActions.THIRST;
    }

    private string _makeFaithDecision()
    {
        return _isMasked(NpcActions.RELIGION_CHURCH) ? null : NpcActions.RELIGION_CHURCH;
    }

    private string _makeCommunicationDecision()
    {
        return _isMasked(NpcActions.COMMUNICATION_CHAT) ? null : NpcActions.COMMUNICATION_CHAT;
    }

    private string _makeRecreationDecision()
    {
        if (_npcState.Recreation > 0.2f) return null;

        List<KeyValuePair<string, float>> attributes = new List<KeyValuePair<string, float>>();
        foreach (string attType in new string[] { NpcState.ATT_COMPANIONSHIP, NpcState.ATT_FITNESS,
            NpcState.ATT_COMMODITIES, NpcState.ATT_FEROCITY, NpcState.ATT_INTELLIGENCE })
        {
            attributes.Add(new KeyValuePair<string, float>(attType, _npcState.getAttributeByType(attType)));
        }
        attributes.Sort((x, y) => y.Value.CompareTo(x.Value));

        List<string> attributeActions = new List<string>();
        foreach (KeyValuePair<string, float> att in attributes)
        {
            string preferredAction = _getPreferredRecreationAction(att.Key);
            if (preferredAction == null) continue;
            if (!_isMasked(preferredAction)) attributeActions.Add(preferredAction);
        }
        if (attributeActions.Count == 0) return null;
        return attributeActions[Mathf.RoundToInt(UnityEngine.Random.value * (attributeActions.Count - 1))];
    }

    private string _getPreferredRecreationAction(string attribute)
    {
        string action = AttributeHandler.getPreferredAttributeAction(attribute, _mask);
        if (action == null)
        {
            if (GameAcademy.VERBOSE) Debug.LogWarning("No preferred recreation action found for attribute " + attribute + ".");
            string[] actions = NpcActions.RECREATION_ACTIONS;
            string noAttAction = actions[Mathf.RoundToInt(Random.value * (actions.Length - 1))];
            return noAttAction;
        }
        return action;
    }
}