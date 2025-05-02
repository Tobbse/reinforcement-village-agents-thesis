using System;

public class WorkBreadBakeryActionPlan : WorkActionPlan
{
    public const int ACTION_CREATE_BREAD = 12;
    public const int ACTION_CONSUME_FLOUR = 5;
    public const int ACTION_CONSUME_TOOLS = 1;

    public WorkBreadBakeryActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.WORK_BAKERY, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return  _hasEnoughResource(ResourceHandler.FLOUR, ACTION_CONSUME_FLOUR) &&
                _hasEnoughResource(ResourceHandler.TOOLS, ACTION_CONSUME_TOOLS) &&
                _hasResourceCapacity(ResourceHandler.BREAD, ACTION_CREATE_BREAD) &&
                _hasFreeSpot(LocationTypes.BAKERY);
    }

    protected override void _addPlanActions()
    {
        float seconds = TimeController.gameSecondsFromRealHours(4.5f);

        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS, LocationTypes.BAKERY);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_MAIN, true);
        _moveAndWaitRealSeconds(seconds, LocationTypes.BAKERY);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_MAIN, true);
    }

    protected override void _initRun()
    {
        base._initRun();
        _resourceHandler.useResource(ACTION_CONSUME_FLOUR, ResourceHandler.FLOUR);
        _resourceHandler.useResource(ACTION_CONSUME_TOOLS, ResourceHandler.TOOLS);
    }

    protected override void _additionalFinishAction()
    {
        _resourceHandler.addResource(ACTION_CREATE_BREAD, ResourceHandler.BREAD);
        _npcState.gainMoney(HIGH_CLASS_WORK_MONEY_REWARD);
    }
}
