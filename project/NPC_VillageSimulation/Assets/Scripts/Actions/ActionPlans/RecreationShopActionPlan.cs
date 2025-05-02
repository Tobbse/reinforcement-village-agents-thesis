using System;

public class RecreationShopActionPlan : BaseActionPlan
{
    public RecreationShopActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.RECREATION_SHOP, allActionsExecutedCallback, npc)
    {
        _money = 0.2f;
    }

    public override bool areRequirementsMet()
    {
        if (!_hasEnoughMoney(_money)) return false;
        if (!_hasFreeSpot(LocationTypes.SHOP)) return false;
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS * 2f, LocationTypes.SHOP);
    }

    protected override float _getReward()
    {
        float reward = _getRecreationReward(RECREATION_REWARD_MULTIPLIER);
        reward = _calculateAttributeMultReward(NpcState.ATT_COMMODITIES, reward);
        reward = _calculateAttributeMultReward(NpcState.ATT_FEROCITY, reward, true);
        return reward > 0 ? reward * 0.9f : reward;
    }
}
