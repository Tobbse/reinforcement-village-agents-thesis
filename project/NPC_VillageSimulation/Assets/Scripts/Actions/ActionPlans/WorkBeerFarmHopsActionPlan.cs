using System;

public class WorkBeerFarmHopsActionPlan : WorkActionPlan
{
    public const int ACTION_CREATE_HOPS = 2;
    public const int ACTION_CONSUME_TOOLS = 1;

    public WorkBeerFarmHopsActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.WORK_HOPS_FARMING, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return  _hasEnoughResource(ResourceHandler.TOOLS, ACTION_CONSUME_TOOLS) &&
                _hasResourceCapacity(ResourceHandler.HOPS, ACTION_CREATE_HOPS) &&
                _hasFreeSpot(LocationTypes.FIELD_HOPS) /*&&
                _npc.VillageInfo.HopsFieldsDestroyed*/;
    }

    protected override void _addPlanActions()
    {
        float seconds = TimeController.gameSecondsFromRealHours(5.5f);

        _moveAndWaitRealSeconds(seconds, LocationTypes.FIELD_HOPS);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_HOPS, true);
    }

    protected override void _initRun()
    {
        base._initRun();
        _resourceHandler.useResource(ACTION_CONSUME_TOOLS, ResourceHandler.TOOLS);
    }

    protected override void _additionalFinishAction()
    {
        _resourceHandler.addResource(ACTION_CREATE_HOPS, ResourceHandler.HOPS);
        _npcState.gainMoney(LOW_CLASS_WORK_MONEY_REWARD);
    }
}
