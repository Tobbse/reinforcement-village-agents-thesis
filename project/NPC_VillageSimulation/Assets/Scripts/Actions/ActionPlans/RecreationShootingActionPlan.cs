using System;

public class RecreationShootingActionPlan : BaseActionPlan
{
    public RecreationShootingActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.RECREATION_SHOOTING, allActionsExecutedCallback, npc)
    {
        _money = 0.1f;
    }

    public override bool areRequirementsMet()
    {
        if (!_hasEnoughMoney(_money)) return false;
        if (!_hasFreeSpot(LocationTypes.SHOOTING_RANGE)) return false;
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS * 1.3f, LocationTypes.SHOOTING_RANGE);
    }

    protected override float _getReward()
    {
        float reward = _getRecreationReward(RECREATION_REWARD_MULTIPLIER);
        reward = _calculateAttributeMultReward(NpcState.ATT_FEROCITY, reward);
        reward = _calculateAttributeMultReward(NpcState.ATT_COMPANIONSHIP, reward, true);
        return reward > 0 ? reward * 1.15f : reward;
    }
}
