using System;

public class WorkBreadWindmillActionPlan : WorkActionPlan
{
    public const int ACTION_CREATE_FLOUR = 5;
    public const int ACTION_CONSUME_GRAIN = 5;
    public const int ACTION_CONSUME_TOOLS = 1;

    public WorkBreadWindmillActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.WORK_WINDMILL, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return  _hasEnoughResource(ResourceHandler.GRAIN, ACTION_CONSUME_GRAIN) &&
                _hasEnoughResource(ResourceHandler.TOOLS, ACTION_CONSUME_TOOLS) &&
                _hasResourceCapacity(ResourceHandler.FLOUR, ACTION_CREATE_FLOUR) &&
                _hasFreeSpot(LocationTypes.WINDMILL);
    }

    protected override void _addPlanActions()
    {
        float seconds = TimeController.gameSecondsFromRealHours(5f);

        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_GRAIN, true);
        _moveAndWaitRealSeconds(seconds, LocationTypes.WINDMILL);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_MAIN, true);
    }

    protected override void _initRun()
    {
        base._initRun();
        _resourceHandler.useResource(ACTION_CONSUME_GRAIN, ResourceHandler.GRAIN);
        _resourceHandler.useResource(ACTION_CONSUME_TOOLS, ResourceHandler.TOOLS);
    }

    protected override void _additionalFinishAction()
    {
        _resourceHandler.addResource(ACTION_CREATE_FLOUR, ResourceHandler.FLOUR);
        _npcState.gainMoney(HIGH_CLASS_WORK_MONEY_REWARD);
    }
}
