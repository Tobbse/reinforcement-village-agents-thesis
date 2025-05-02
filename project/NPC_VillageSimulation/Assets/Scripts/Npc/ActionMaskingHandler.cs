using System.Collections.Generic;

public class ActionMaskingHandler
{
    private LocationHandler _locationHandler;
    private ResourceHandler _resourceHandler;
    private JobHandler _jobHandler;
    private VillageInfo _villageInfo;
    private ActionHandler _actionHandler;
    private HashSet<string> _actionMask;
    private Dictionary<string, int> _actionMaskLog;
    private Dictionary<string, int> _actionMaskLogWithoutSameActions;

    /**
     * Masks actions that should be unavailable for the next decision,
     * for example when the agent has insufficient money.
     */
    public ActionMaskingHandler(NpcAgent npc)
    {
        _locationHandler = npc.LocationHandler;
        _resourceHandler = npc.ResourceHandler;
        _jobHandler = npc.JobHandler;
        _villageInfo = npc.VillageInfo;
        _actionHandler = npc.ActionHandler;

        if (GameAcademy.LOG_ACTION_MASK_AMOUNTS)
        {
            _actionMaskLog = new Dictionary<string, int>();
            _actionMaskLogWithoutSameActions = new Dictionary<string, int>();
            foreach (string actionId in NpcActions.ALL_ACTIONS) _actionMaskLog[actionId] = 0;
            foreach (string actionId in NpcActions.ALL_ACTIONS) _actionMaskLogWithoutSameActions[actionId] = 0;
        }
    }

    public List<string> getStringActionMask(NpcAgent npc)
    {
        List<string> listMask = new List<string>();
        HashSet<string> hashMask = _getMaskHashSet(npc);
        foreach (string actionId in hashMask) listMask.Add(actionId);
        return listMask;
    }

    public List<int> getIntActionMask(NpcAgent npc)
    {
        HashSet<string> hashMask = _getMaskHashSet(npc);
        return _createIntMask(hashMask);
    }

    private HashSet<string> _getMaskHashSet(NpcAgent npc)
    {
        _actionMask = new HashSet<string>();

        foreach (string actionId in NpcActions.ALL_ACTIONS)
        {
            if (!_actionHandler.areRequirementsMetForAction(actionId)) _actionMask.Add(actionId);
        }

        _addJobMask(npc.NpcState.AgentInfo.JobId);

        bool containsLastAction = false;
        if (GameAcademy.LOG_ACTION_MASK_AMOUNTS)
        {
            foreach (string actionId in _actionMask)
            {
                _actionMaskLogWithoutSameActions[actionId] += 1;
                _actionMaskLog[actionId] += 1;
                if (actionId == npc.Action) containsLastAction = true;
            }
        }
        if (npc.Action != "")
        {
            _actionMask.Add(npc.Action); // Do not execute the same action again.
            if (GameAcademy.LOG_ACTION_MASK_AMOUNTS && containsLastAction == false)
            {
                _actionMaskLog[npc.Action] += 1;
            }
        }
        return _actionMask;
    }

    private List<int> _createIntMask(HashSet<string> actionMask)
    {
        List<int> intActionMask = new List<int>();
        foreach (string action in actionMask) intActionMask.Add(NpcActions.actionIdFromName(action));
        return intActionMask;
    }

    // Prevent agent from doing any jobs other than his own.
    private void _addJobMask(string agentJobId)
    {
        if (GameAcademy.IGNORE_JOB_MASK) return;
        string agentWorkId = _jobHandler.getActionIdFromJobId(agentJobId);

        foreach (string workId in NpcActions.WORK_ACTIONS)
        {
            if (workId == agentWorkId) continue;
            else _actionMask.Add(workId);
        }
    }
}
