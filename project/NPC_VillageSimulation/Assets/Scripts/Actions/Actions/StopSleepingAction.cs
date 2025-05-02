using UnityEngine;
using System;
using System.Collections;

public class StopSleepingAction : BaseAction
{
    public StopSleepingAction(string actionType, NpcAgent npc)
        : base(actionType, npc) { }

    public override IEnumerator execute(Action callback, Action cancelCallback)
    {
        //_npc.NpcState.BlockNeedUpdates = false;
        _npc.GetComponentInChildren<MeshRenderer>().enabled = true;
        _npc.NpcState.updateExhaustion(1f);
        callback();
        yield return null;
    }
}
