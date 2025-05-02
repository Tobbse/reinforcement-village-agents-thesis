using System;

public class WorkMeatButcherActionPlan : WorkActionPlan
{
    public const int ACTION_CREATE_COOKED_MEAT = 14;
    public const int ACTION_CONSUME_RAW_VENISON = 8;
    public const int ACTION_CONSUME_TOOLS = 1;

    public WorkMeatButcherActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.WORK_BUTCHER, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return _hasEnoughResource(ResourceHandler.RAW_VENISON, ACTION_CONSUME_RAW_VENISON) &&
               _hasEnoughResource(ResourceHandler.TOOLS, ACTION_CONSUME_TOOLS) &&
               _hasResourceCapacity(ResourceHandler.COOKED_MEAT, ACTION_CREATE_COOKED_MEAT) &&
               _hasFreeSpot(LocationTypes.BUTCHER);
    }

    protected override void _addPlanActions()
    {
        float seconds = TimeController.gameSecondsFromRealHours(4f);

        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS, LocationTypes.BUTCHER);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_FOREST, true);
        _moveAndWaitRealSeconds(seconds, LocationTypes.BUTCHER);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_MAIN, true);
    }

    protected override void _initRun()
    {
        base._initRun();
        _resourceHandler.useResource(ACTION_CONSUME_RAW_VENISON, ResourceHandler.RAW_VENISON);
        _resourceHandler.useResource(ACTION_CONSUME_TOOLS, ResourceHandler.TOOLS);
    }

    protected override void _additionalFinishAction()
    {
        _resourceHandler.addResource(ACTION_CREATE_COOKED_MEAT, ResourceHandler.COOKED_MEAT);
        _npcState.gainMoney(HIGH_CLASS_WORK_MONEY_REWARD);
    }
}
