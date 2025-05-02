using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SwordGC.AI.Goap;

public class VillageInfo
{
    private int numAgentsMoving = 0;
    private int numAgentsWaiting = 0;
    private Dictionary<string, int> _currentActionAmounts;
    private int _villageId;
    private bool _trackJobAmountsInUI;
    private Dictionary<string, Text> _jobAmountTexts;
    private Dictionary<string, int> _jobAmounts;
    private Dictionary<string, List<float>> _actionDurations;
    private Dictionary<string, float> _averageActionDurations;
    private Dictionary<string, List<Dictionary<string, float>>> _actionAttributes;
    private bool _showWorkAmounts;

    public int VillageId { get => _villageId; }

    public VillageInfo(int villageId, Dictionary<string, int> jobAmounts,
                       Dictionary<string, Text> jobAmountTexts, bool showWorkAmounts)
    {
        _villageId = villageId;
        _jobAmounts = jobAmounts;
        _jobAmountTexts = jobAmountTexts;
        _trackJobAmountsInUI = _jobAmountTexts != null;
        _currentActionAmounts = new Dictionary<string, int>();
        _showWorkAmounts = showWorkAmounts;

        if (GameAcademy.LOG_ACTION_DURATIONS) {
            _actionDurations = new Dictionary<string, List<float>>();
            if (GameAcademy.DEBUG_MODE) _averageActionDurations = new Dictionary<string, float>();
        }
        if (GameAcademy.LOG_ATTRIBUTES)
        {
            _actionAttributes = new Dictionary<string, List<Dictionary<string, float>>>();
            foreach (string actionId in NpcActions.ALL_ACTIONS) _actionAttributes[actionId] = new List<Dictionary<string, float>>();
        }
        if (!GameAcademy.USE_GOAP) foreach (string action in NpcActions.ALL_ACTIONS) _currentActionAmounts[action] = 0;
    }

    public void changeJobAmount(int changeBy, string job)
    {
        if (!_showWorkAmounts) return;
        int amount = _jobAmounts[job] += changeBy;
        _jobAmounts[job] = amount;
        if (!_trackJobAmountsInUI) return;
        _jobAmountTexts[job].text = amount.ToString();
    }

    public void addActionDuration(string actionId, float duration)
    {
        if (!_actionDurations.ContainsKey(actionId))
        {
            _actionDurations[actionId] = new List<float>();
        }
        _actionDurations[actionId].Add(duration);

        if (GameAcademy.DEBUG_MODE && _actionDurations[actionId].Count % 10 == 0)
        {
            float sum = 0f;
            foreach (float dur in _actionDurations[actionId]) sum += dur;
            _averageActionDurations[actionId] = sum / _actionDurations[actionId].Count;
        }
    }

    public float getNumWorkersForJobNormalized(string job)
    {
        return (float)_jobAmounts[job] / (float)GameAcademy.NUM_AGENTS_PER_VILLAGE;
    }

    public void changeAmountForAction(bool add, string action)
    {
        int changeBy = add ? 1 : -1;
        if (!_currentActionAmounts.ContainsKey(action)) _currentActionAmounts[action] = 0;
        _currentActionAmounts[action] += changeBy;

        GameMonitor.LogToPos(action + "", _currentActionAmounts[action].ToString(), GameMonitor.LOG_LOCATION_TOP_LEFT);
    }

    public void changeAmountForActionToZero(string action)
    {
        _currentActionAmounts[action] = 0;
        GameMonitor.LogToPos(action + "", _currentActionAmounts[action].ToString(), GameMonitor.LOG_LOCATION_TOP_LEFT);
    }

    public void changeAmountMoving(bool add)
    {
        numAgentsMoving += add ? 1 : -1;
        GameMonitor.LogToPos("Moving", numAgentsMoving.ToString(), GameMonitor.LOG_LOCATION_TOP_LEFT);
    }

    public void changeAmountWaiting(bool add)
    {
        numAgentsWaiting += add ? 1 : -1;
        GameMonitor.LogToPos("Waiting", numAgentsWaiting.ToString(), GameMonitor.LOG_LOCATION_TOP_LEFT);
    }

    public int getAmountAgentsForAction(string action)
    {
        if (!_currentActionAmounts.ContainsKey(action)) return 0;
        return _currentActionAmounts[action];
    }

    public void addAgentAttributesForAction(string executedActionId, Dictionary<string, float> attributes)
    {
        _actionAttributes[executedActionId].Add(attributes);
        bool calculateAverage = false; // Can be set to true by debugger to calculate averages.
        if (calculateAverage)
        {
            Dictionary<string, Dictionary<string, float>> averages = new Dictionary<string, Dictionary<string, float>>();
            foreach (string actionId in _actionAttributes.Keys)
            {
                Dictionary<string, float> averageAtts = new Dictionary<string, float>();
                averageAtts[NpcState.ATT_COMMODITIES] = averageAtts[NpcState.ATT_COMPANIONSHIP] = averageAtts[NpcState.ATT_FEROCITY] = averageAtts[NpcState.ATT_FITNESS] = averageAtts[NpcState.ATT_INTELLIGENCE] = 0f;
                foreach (Dictionary<string, float> loggedAttDict in _actionAttributes[actionId]) // Iterate over list of dicts.
                {
                    foreach (string attributeId in loggedAttDict.Keys) // Accumulate attributes.
                    {
                        averageAtts[attributeId] += loggedAttDict[attributeId];
                    }
                }
                foreach (string attributeId in new List<string>(averageAtts.Keys)) // Calculate averages.
                {
                    averageAtts[attributeId] /= _actionAttributes[actionId].Count;
                }
                averages[actionId] = averageAtts;
            }
        }
    }
}