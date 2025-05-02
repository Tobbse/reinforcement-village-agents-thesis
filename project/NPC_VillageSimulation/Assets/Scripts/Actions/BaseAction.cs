using System;
using UnityEngine;
using System.Collections;

public class BaseAction
{
    protected string _actionType;
    protected NpcAgent _npc;
    protected TimeController _timeController;

    public BaseAction(string actionType, NpcAgent npc)
    {
        _actionType = actionType;
        _npc = npc;
        _timeController = GameObject.FindObjectOfType<TimeController>();
    }

    public virtual IEnumerator execute(Action callback, Action cancelCallback)
    {
        throw new System.NotImplementedException("Function execute not implemented for " + this.GetType());
    }
}
