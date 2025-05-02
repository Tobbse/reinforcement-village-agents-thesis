using System;

public class WorkWoodLoggingActionPlan : WorkActionPlan
{
    public const int ACTION_CREATE_WOODEN_LOGS = 2;
    public const int ACTION_CONSUME_TOOLS = 1;

    public WorkWoodLoggingActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.WORK_LOGGING, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return  _hasEnoughResource(ResourceHandler.TOOLS, ACTION_CONSUME_TOOLS) &&
                _hasResourceCapacity(ResourceHandler.WOODEN_LOG, ACTION_CREATE_WOODEN_LOGS) &&
                _hasFreeSpot(LocationTypes.WOOD_CUTTING_SPOT);
    }

    protected override void _addPlanActions()
    {
        float seconds = TimeController.gameSecondsFromRealHours(5f);

        _moveAndWaitRealSeconds(seconds, LocationTypes.WOOD_CUTTING_SPOT, false,
            MoveAction.PICK_BUILDING_STRATEGY_RANDOM);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_FOREST, true);
    }

    protected override void _initRun()
    {
        base._initRun();
        _resourceHandler.useResource(ACTION_CONSUME_TOOLS, ResourceHandler.TOOLS);
    }

    protected override void _additionalFinishAction()
    {
        _resourceHandler.addResource(ACTION_CREATE_WOODEN_LOGS, ResourceHandler.WOODEN_LOG);
        _npcState.gainMoney(LOW_CLASS_WORK_MONEY_REWARD);
    }
}
