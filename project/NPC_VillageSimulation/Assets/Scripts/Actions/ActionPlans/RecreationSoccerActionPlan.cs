using System;

public class RecreationSoccerActionPlan : BaseActionPlan
{
    public RecreationSoccerActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.RECREATION_SOCCER, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        if (!_hasFreeSpot(LocationTypes.SOCCER_FIELD)) return false;
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS * 2f, LocationTypes.SOCCER_FIELD,
            false, MoveAction.PICK_BUILDING_STRATEGY_RANDOM);
    }

    protected override float _getReward()
    {
        float reward = _getRecreationReward(RECREATION_REWARD_MULTIPLIER);
        reward = _calculateAttributeMultReward(NpcState.ATT_FITNESS, reward);
        reward = _calculateAttributeMultReward(NpcState.ATT_FEROCITY, reward);
        return reward  > 0 ? reward * 0.85f : reward;
    }
}
