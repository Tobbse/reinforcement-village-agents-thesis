using System;

public class RecreationBarbecueActionPlan : BaseActionPlan
{
    public RecreationBarbecueActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.RECREATION_BARBECUE, allActionsExecutedCallback, npc)
    {
        _money = 0.01f;
    }

    public override bool areRequirementsMet()
    {
        if (!_hasEnoughMoney(_money)) return false;
        if (!_hasFreeSpot(LocationTypes.BARBECUE)) return false;
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS * 1.3f, LocationTypes.BARBECUE);
    }

    protected override float _getReward()
    {
        float reward = _getRecreationReward(RECREATION_REWARD_MULTIPLIER);
        reward = _calculateAttributeMultReward(NpcState.ATT_COMPANIONSHIP, reward);
        return _calculateAttributeMultReward(NpcState.ATT_FITNESS, reward, true);
    }
}
