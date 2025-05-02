using System;

public class HungerBreadActionPlan : BaseActionPlan
{
    public const int ACTION_CONSUME_BREAD = 1;

    public HungerBreadActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.HUNGER_BREAD, allActionsExecutedCallback, npc)
    {
        _money = 0.1f;
    }

    public override bool areRequirementsMet()
    {
        if (!_hasEnoughMoney(_money)) return false;
        if (!_hasEnoughResource(ResourceHandler.BREAD, ACTION_CONSUME_BREAD)) return false;
        if (!_hasFreeSpot(LocationTypes.MARKET)) return false;
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS * 0.5f, LocationTypes.MARKET);
    }

    protected override void _initRun()
    {
        base._initRun();
        _npc.ResourceHandler.useResource(ACTION_CONSUME_BREAD, ResourceHandler.BREAD);
    }

    protected override float _getReward()
    {
        return _getNeedReward(NpcState.NEED_HUNGER, NEED_REWARD_MULTIPLIER);
    }

    protected override void _additionalFinishAction()
    {
        _npcState.satisfyNeedByType(NpcState.NEED_HUNGER, DEFAULT_NEED_GAIN);
    }
}
