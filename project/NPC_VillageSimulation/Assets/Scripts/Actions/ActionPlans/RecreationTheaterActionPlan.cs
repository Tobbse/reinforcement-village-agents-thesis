using System;

public class RecreationTheaterActionPlan : BaseActionPlan
{
    public RecreationTheaterActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.RECREATION_THEATER, allActionsExecutedCallback, npc)
    {
        _money = 0.05f;
    }

    public override bool areRequirementsMet()
    {
        if (!_hasEnoughMoney(_money)) return false;
        if (!_hasFreeSpot(LocationTypes.THEATER)) return false;
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS * 2f, LocationTypes.THEATER);
    }

    protected override float _getReward()
    {
        float reward = _getRecreationReward(RECREATION_REWARD_MULTIPLIER);
        reward = _calculateAttributeMultReward(NpcState.ATT_INTELLIGENCE, reward);
        return reward > 0 ? reward * 0.95f : reward;
    }
}
