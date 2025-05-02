using System;

public class WorkWoodCarpenterActionPlan : WorkActionPlan
{
    public const int ACTION_CREATE_FIREFOOD = 30;
    public const int ACTION_CONSUME_WOODEN_LOGS = 8;
    public const int ACTION_CONSUME_TOOLS = 1;

    public WorkWoodCarpenterActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.WORK_CARPENTER, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return  _hasEnoughResource(ResourceHandler.WOODEN_LOG, ACTION_CONSUME_WOODEN_LOGS) &&
                _hasEnoughResource(ResourceHandler.TOOLS, ACTION_CONSUME_TOOLS) &&
                _hasResourceCapacity(ResourceHandler.FIREWOOD, ACTION_CREATE_FIREFOOD) &&
                _hasFreeSpot(LocationTypes.CARPENTER);
    }

    protected override void _addPlanActions()
    {
        float seconds = TimeController.gameSecondsFromRealHours(4.5f);

        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS, LocationTypes.CARPENTER);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_FOREST, true, MoveAction.PICK_BUILDING_STRATEGY_RANDOM);
        _moveAndWaitRealSeconds(seconds, LocationTypes.CARPENTER);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_MAIN, true);
    }

    protected override void _initRun()
    {
        base._initRun();
        _resourceHandler.useResource(ACTION_CONSUME_WOODEN_LOGS, ResourceHandler.WOODEN_LOG);
        _resourceHandler.useResource(ACTION_CONSUME_TOOLS, ResourceHandler.TOOLS);
    }

    protected override void _additionalFinishAction()
    {
        _resourceHandler.addResource(ACTION_CREATE_FIREFOOD, ResourceHandler.FIREWOOD);
        _npcState.gainMoney(HIGH_CLASS_WORK_MONEY_REWARD);
    }
}
