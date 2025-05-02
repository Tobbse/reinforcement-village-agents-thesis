using System;

public class ReligionChurchActionPlan : BaseActionPlan
{
    public ReligionChurchActionPlan(Action allActionsExecutedCallback,
                                   NpcAgent npc)
        : base(NpcActions.RELIGION_CHURCH, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        if (!_hasFreeSpot(LocationTypes.CHURCH)) return false;
        return true;
    }

    protected override float _getReward()
    {
        return _getNeedReward(NpcState.NEED_FAITH, NEED_REWARD_MULTIPLIER);
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS, LocationTypes.CHURCH);
    }

    protected override void _additionalFinishAction()
    {
        _npcState.satisfyNeedByType(NpcState.NEED_FAITH, DEFAULT_NEED_GAIN);
    }
}
