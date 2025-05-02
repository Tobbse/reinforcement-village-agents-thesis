using System;

public class ThirstActionPlan : BaseActionPlan
{
    public ThirstActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.THIRST, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        if (!_hasFreeSpot(LocationTypes.WELL))
        {
            return false;
        }
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS / 5f, LocationTypes.WELL);
    }

    protected override float _getReward()
    {
        return _getNeedReward(NpcState.NEED_THIRST, NEED_REWARD_MULTIPLIER);
    }

    protected override void _additionalFinishAction()
    {
        _npcState.satisfyNeedByType(NpcState.NEED_THIRST, DEFAULT_NEED_GAIN);
    }
}
