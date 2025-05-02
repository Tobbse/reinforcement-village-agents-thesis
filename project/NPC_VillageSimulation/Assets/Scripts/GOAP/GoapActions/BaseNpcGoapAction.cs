using SwordGC.AI.Goap;
using System.Collections;
using CielaSpike;
using UnityEngine;
using System.Collections.Generic;
using System;

public class BaseNpcGoapAction : GoapAction
{
    public static List<string> ALL_GOAP_ACTIONS;

    protected string _locationType;
    protected NpcAgent _npc;
    protected bool _isPerforming;

    public bool IsPerforming { get => _isPerforming; }

    public BaseNpcGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent)
    {
        _npc = npc;
        // Increasing this value to make sure that the agent is actually stopped, as calculations
        // for the NavMeshAgent stopping distance and the requiredRange might differ.
        requiredRange = GameAcademy.AGENT_STOPPING_DISTANCE * 1.5f;
        _init();
    }

    public override void OnStart()
    {
        if (target == null || target == _npc.gameObject)
        {
            target = _npc.gameObject;
            _npc.StartCoroutine(addTarget());
        }
        base.OnStart();
    }

    public IEnumerator addTarget()
    {
        yield return Ninja.JumpToUnity;
        yield return _npc.LocationHandler.getRandomGoapPosition(_locationType, _npc.AgentId, originalObjectGUID);
        var result = _npc.LocationHandler.getcalculatedGoapPosition(originalObjectGUID);
        if (result == null) target = _npc.HomePosition.gameObject;
        else target = result.gameObject;
    }

    public override void Perform()
    {
        _npc.Action = GetType().ToString();
        _isPerforming = false;
    }

    protected void _waitForIngameSeconds(float ingameSeconds)
    {
        _npc.VillageInfo.changeAmountWaiting(true);
        _isPerforming = true;

        Task task;
        _npc.StartCoroutineAsync(Wait(ingameSeconds / Time.timeScale), out task);
    }

    private IEnumerator Wait(float seconds)
    {
        if (GameAcademy.VERBOSE) Debug.Log(GetType().ToString() + " now waiting for " + seconds.ToString() + " seconds.");
        yield return new WaitForSecondsRealtime(seconds);
        _isPerforming = false;
        _npc.VillageInfo.changeAmountWaiting(false);
        yield return null;
    }

    protected virtual void _init()
    {
        throw new System.NotImplementedException("_init is not implemented on " + this.GetType());
    }

    protected virtual BaseNpcGoapAction _getDirectClone()
    {
        return new BaseNpcGoapAction(this.agent, _npc);
    }

    public override GoapAction Clone ()
    {
        BaseNpcGoapAction action = _getDirectClone();
        string newGuid = Guid.NewGuid().ToString();
        action.target = this.target; // Assigning the current target first, so that some target is available.
        //_npc.LocationHandler.addPendingTargetGoapAction(action); // This will update the target safely on the main thread.
        return action.SetClone(newGuid); // New GUID is necessary to not confuse the new action with this or other child actions.
    }
}