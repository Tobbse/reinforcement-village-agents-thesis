using System;

public class WorkToolsBlacksmithActionPlan : WorkActionPlan
{
    public const int ACTION_CREATE_TOOLS = 23;
    public const int ACTION_CONSUME_ORE = 8;

    public WorkToolsBlacksmithActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.WORK_BLACKSMITH, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return  _hasEnoughResource(ResourceHandler.ORE, ACTION_CONSUME_ORE) &&
                _hasResourceCapacity(ResourceHandler.TOOLS, ACTION_CREATE_TOOLS) &&
                _hasFreeSpot(LocationTypes.BLACKSMITH);
    }

    protected override void _addPlanActions()
    {
        float seconds = TimeController.gameSecondsFromRealHours(4f);
        _moveAndWaitRealSeconds(5f, LocationTypes.BLACKSMITH);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_MAIN, true);
        _moveAndWaitRealSeconds(5f, LocationTypes.BLACKSMITH);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_MAIN, true);
        _moveAndWaitRealSeconds(seconds, LocationTypes.BLACKSMITH);
        _moveAndWaitRealSeconds(3f, LocationTypes.STORAGE_MAIN, true);
    }

    protected override void _initRun()
    {
        base._initRun();
        _resourceHandler.useResource(ACTION_CONSUME_ORE, ResourceHandler.ORE);
    }

    protected override void _additionalFinishAction()
    {
        _resourceHandler.addResource(ACTION_CREATE_TOOLS, ResourceHandler.TOOLS);
        _npcState.gainMoney(HIGH_CLASS_WORK_MONEY_REWARD);
    }
}
