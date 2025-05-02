using System;

public class CommunicationChatActionPlan : BaseActionPlan
{
    public CommunicationChatActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.COMMUNICATION_CHAT, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        if (!_hasFreeSpot(LocationTypes.MEETING_POINT)) return false;
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS * 1.5f, LocationTypes.MEETING_POINT,
            false, MoveAction.PICK_BUILDING_STRATEGY_NEXT_OCCUPIED);
    }

    protected override float _getReward()
    {
        return _getNeedReward(NpcState.NEED_COMMUNICATION, NEED_REWARD_MULTIPLIER);
    }

    protected override void _additionalFinishAction()
    {
        _npcState.satisfyNeedByType(NpcState.NEED_COMMUNICATION, DEFAULT_NEED_GAIN);
    }
}
