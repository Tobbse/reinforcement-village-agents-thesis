using System;

public class ExhaustionRestActionPlan : BaseActionPlan
{
    private float _initExhaustion;

    public ExhaustionRestActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.RESTING, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS * 2f, LocationTypes.HOME, true, MoveAction.PICK_BUILDING_STRATEGY_HOME);
    }

    protected override void _initRun()
    {
        base._initRun();
        _initExhaustion = _npc.NpcState.Exhaustion;
    }

    protected override float _getReward()
    {
        return (NEED_PENALTY_THRESHOLD - _initExhaustion) * NEED_REWARD_MULTIPLIER;
    }

    protected override void _additionalFinishAction()
    {
        _npc.NpcState.updateExhaustion(0.6f);
    }
}
