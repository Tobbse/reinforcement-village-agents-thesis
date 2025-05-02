using System;

public class RecreationCafeActionPlan : BaseActionPlan
{
    public RecreationCafeActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.RECREATION_CAFE, allActionsExecutedCallback, npc)
    {
        _money = 0.05f;
    }

    public override bool areRequirementsMet()
    {
        if (!_hasEnoughMoney(_money)) return false;
        if (!_hasFreeSpot(LocationTypes.CAFE)) return false;
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS * 1.3f, LocationTypes.CAFE);
    }

    protected override float _getReward()
    {
        float reward = _getRecreationReward(RECREATION_REWARD_MULTIPLIER);
        reward = _calculateAttributeMultReward(NpcState.ATT_COMPANIONSHIP, reward);
        return _calculateAttributeMultReward(NpcState.ATT_FEROCITY, reward, true);
    }
}
