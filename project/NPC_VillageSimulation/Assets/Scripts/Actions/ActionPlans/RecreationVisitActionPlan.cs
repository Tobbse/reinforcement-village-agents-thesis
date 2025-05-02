using System;

public class RecreationVisitActionPlan : BaseActionPlan
{
    public RecreationVisitActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.RECREATION_VISIT, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return true;
    }

    protected override void _addPlanActions()
    {
        // Basically moves to the next home building that has an occupied slot to visit another npcs. Goes to its own home and waits for visitors if there are no occupied buildings.
        // Because every other npcs with this action will also move to the same house until all slots are full, this will allow for small "parties" to be created.
        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS * 2f, LocationTypes.HOME, true,
            MoveAction.PICK_BUILDING_STRATEGY_NEXT_OCCUPIED, _npc.HomeBuilding);
    }

    protected override float _getReward()
    {
        float reward = _getRecreationReward(RECREATION_REWARD_MULTIPLIER);
        reward = _calculateAttributeMultReward(NpcState.ATT_COMPANIONSHIP, reward);
        reward = _calculateAttributeMultReward(NpcState.ATT_COMMODITIES, reward, true);
        return reward > 0 ? reward * 0.9f : reward;
    }
}
