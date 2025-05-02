using System;
using System.Collections;

public class VacateSlotAction : BaseAction
{
    private string _locationType;

    public VacateSlotAction(string actionType, NpcAgent npc, string locationType)
        : base(actionType, npc)
    {
        _locationType = locationType;
    }

    public override IEnumerator execute(Action callback, Action cancelCallback)
    {
        int agentId = _npc.AgentId;
        _npc.LocationHandler.vacateSlot(_locationType, agentId);
        callback();
        yield return null;
    }
}
