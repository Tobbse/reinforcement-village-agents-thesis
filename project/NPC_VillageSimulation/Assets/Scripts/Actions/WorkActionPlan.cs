using System;

public class WorkActionPlan : BaseActionPlan
{
    protected float _initWork;

    public WorkActionPlan(string actionType, Action allActionsExecutedCallback, NpcAgent npc)
        : base(actionType, allActionsExecutedCallback, npc) { }

    protected override float _getReward()
    {
        return _getWorkReward(WORK_REWARD_MULTIPLIER);
    }

    protected override void _initRun()
    {
        base._initRun();
        _initWork = _npc.NpcState.Work;
    }

    protected float _getWorkReward(float multiplier)
    {
        return (NEED_PENALTY_THRESHOLD - _initWork) * multiplier;
    }
}