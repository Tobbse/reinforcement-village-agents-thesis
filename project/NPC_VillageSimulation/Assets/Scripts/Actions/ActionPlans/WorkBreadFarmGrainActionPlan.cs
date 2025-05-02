using System;

public class WorkBreadFarmGrainActionPlan : WorkActionPlan
{
    public const int ACTION_CREATE_GRAIN = 2;
    public const int ACTION_CONSUME_TOOLS = 1;

    public WorkBreadFarmGrainActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.WORK_GRAIN_FARMING, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return  _hasEnoughResource(ResourceHandler.TOOLS, ACTION_CONSUME_TOOLS) &&
                _hasResourceCapacity(ResourceHandler.GRAIN, ACTION_CREATE_GRAIN) &&
                _hasFreeSpot(LocationTypes.FIELD_GRAIN) /*&&
                _npc.VillageInfo.GrainFieldsDestroyed*/;
    }

    protected override void _addPlanActions()
    {
        float seconds = TimeController.gameSecondsFromRealHours(5.5f);

        _moveAndWaitRealSeconds(seconds, LocationTypes.FIELD_GRAIN);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_GRAIN, true);
    }

    protected override void _initRun()
    {
        base._initRun();
        _resourceHandler.useResource(ACTION_CONSUME_TOOLS, ResourceHandler.TOOLS);
    }

    protected override void _additionalFinishAction()
    {
        _resourceHandler.addResource(ACTION_CREATE_GRAIN, ResourceHandler.GRAIN);
        _npcState.gainMoney(LOW_CLASS_WORK_MONEY_REWARD);
    }
}
