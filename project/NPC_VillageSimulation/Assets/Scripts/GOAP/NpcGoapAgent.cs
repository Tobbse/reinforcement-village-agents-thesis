using SwordGC.AI.Goap;
using System.Collections.Generic;
using UnityEngine;
using CielaSpike;

public class NpcGoapAgent : GoapAgent
{
    public enum ACTIONSTATE { START, MOVING, PERFORMING }

    private NpcAgent _npcAgent;
    private ACTIONSTATE _cachedActionState;
    private BaseNpcGoapAction _cachedAction;

    protected override bool ActiveActionInRange
    {
        get
        {
            if (ActiveAction == null) return true;
            float distance = Vector3.Distance(transform.position, ActiveAction.target.transform.position);
            return distance <= GameAcademy.AGENT_STOPPING_DISTANCE;
        }
    }

    /*protected void RunAction2()
    {
        if (ActiveAction == null) return; // Agent is still blocked.
        if (_cachedAction == null) _cachedAction = (ActiveAction as BaseNpcGoapAction); // Cached action not set yet.

        //_wasPerforming = _cachedAction.IsPerforming; // The agemt was performing before, but is not performing anymore - therefore we have to
        //if (!_cachedAction.IsPerforming && _wasPerforming) moveCalled = 0; // reset the move called. This is necessary because there is no callback.

        if (_isMoving || _cachedAction.IsPerforming) return; // Action is currently running, skipping.
        else _cachedAction = (ActiveAction as BaseNpcGoapAction); // Cached action has been executed, so we take the new action.

        _cachedAction.Run(Time.deltaTime);

        if (ActiveActionInRange && !_isMoving && _cachedState != STATE.ACTION) // Moving has been completed. Perform action.
        {
            state = STATE.ACTION;
            _cachedState = STATE.ACTION;
            _cachedAction.Perform();
        }

        else if (!_cachedAction.IsPerforming && _cachedState != STATE.MOVING) // Action can not be performed yet. Moving to target.
        {
            state = STATE.MOVING;
            _cachedState = STATE.MOVING;
            Move(_cachedAction);
        }

        else // Something is wrong.
        {
            Debug.LogError("Encountered wrong GOAP action state. No action is performed and the agent is not moving. This should not happen.");
        }
    }*/

    /*protected override void RunAction()
    {
        if (ActiveAction != null && _cachedActionState == ACTIONSTATE.START)
        {
            if (_cachedAction == null && GameAcademy.VERBOSE) Debug.Log("Assigning first GOAP action " + ActiveAction.GetType().ToString() + ".");
            else if (GameAcademy.VERBOSE) Debug.Log("Assigning new GOAP action " +
                ActiveAction.GetType().ToString() + ". Old action: " + _cachedAction.GetType().ToString() + ".");

            _cachedAction = ActiveAction as BaseNpcGoapAction;
            _npcAgent.VillageInfo.changeAmountForAction(true, _cachedAction.GetType().ToString());
            _cachedActionState = ACTIONSTATE.MOVING;
            state = STATE.MOVING;
            Move(_cachedAction);
            return;
        }
        _cachedAction.Run(Time.deltaTime);

        if (_cachedActionState == ACTIONSTATE.MOVING)
        {
            if (_isMoving) return;
            _cachedActionState = ACTIONSTATE.PERFORMING;
            state = STATE.ACTION;
            _cachedAction.Perform();
            return;
        }

        if (_cachedActionState == ACTIONSTATE.PERFORMING)
        {
            if (_cachedAction.IsPerforming) return;
            _npcAgent.VillageInfo.changeAmountForAction(false, _cachedAction.GetType().ToString());
            _cachedActionState = ACTIONSTATE.START;
        }
    }*/

    /*public override void Start()
    {
        _cachedActionState = ACTIONSTATE.START;
    }*/

    public void Init(NpcAgent npcAgent, List<GoapAction> goapActions)
    {
        _npcAgent = npcAgent;
        //activeActions = goapActions;
        foreach (GoapAction action in goapActions) AddAction(action);

        goals = new Dictionary<string, GoapGoal>();
        goals[GoapGoal.Goals.CommunicationGoapGoal] = new CommunicationGoapGoal(GoapGoal.Goals.CommunicationGoapGoal);
        goals[GoapGoal.Goals.EducationGoapGoal] = new EducationGoapGoal(GoapGoal.Goals.EducationGoapGoal);
        goals[GoapGoal.Goals.ExhaustionGoapGoal] = new ExhaustionGoapGoal(GoapGoal.Goals.ExhaustionGoapGoal);
        goals[GoapGoal.Goals.FaithGoapGoal] = new FaithGoapGoal(GoapGoal.Goals.FaithGoapGoal);
        goals[GoapGoal.Goals.HungerGoapGoal] = new HungerGoapGoal(GoapGoal.Goals.HungerGoapGoal);
        goals[GoapGoal.Goals.RecreationGoapGoal] = new RecreationGoapGoal(GoapGoal.Goals.RecreationGoapGoal);
        goals[GoapGoal.Goals.SleepGoapGoal] = new SleepGoapGoal(GoapGoal.Goals.SleepGoapGoal);
        goals[GoapGoal.Goals.ThirstGoapGoal] = new ThirstGoapGoal(GoapGoal.Goals.ThirstGoapGoal);
        goals[GoapGoal.Goals.WorkGoapGoal] = new WorkGoapGoal(GoapGoal.Goals.WorkGoapGoal);

        _blocked = false;
        base.Start();
    }

    protected override void Move(GoapAction nextAction)
    {
        Vector3 moveAgentPosition = _npcAgent.MoveHandler.NavAgent.transform.position;
        Vector3 moveAgentDestination = _npcAgent.MoveHandler.NavAgent.destination;
        Vector3 nextActionDestination = nextAction.target.transform.position;
        if (Equals(moveAgentDestination, nextActionDestination) ||
            Equals(moveAgentPosition, nextActionDestination)) return;

        _npcAgent.VillageInfo.changeAmountMoving(true);
        _npcAgent.MoveHandler.moveAgent(nextAction.target.transform.position, nextAction.GetType().ToString(), null);
    }

    private void _moveFinished()
    {
        _npcAgent.VillageInfo.changeAmountMoving(false);
    }
}