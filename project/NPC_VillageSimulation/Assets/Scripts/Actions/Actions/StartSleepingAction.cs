using UnityEngine;
using System;
using System.Collections;

public class StartSleepingAction : BaseAction
{
    public StartSleepingAction(string actionType, NpcAgent npc)
        : base(actionType, npc) { }

    public override IEnumerator execute(Action callback, Action cancelCallback)
    {
        //_npc.NpcState.BlockNeedUpdates = true;
        _npc.GetComponentInChildren<MeshRenderer>().enabled = false;
        callback();
        yield return null;
    }
}
