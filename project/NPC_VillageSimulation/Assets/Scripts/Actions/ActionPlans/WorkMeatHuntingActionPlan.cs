using System;

public class WorkMeatHuntingActionPlan : WorkActionPlan
{
    public const int ACTION_CREATE_RAW_VENISON = 2;
    public const int ACTION_CONSUME_TOOLS = 1;

    public WorkMeatHuntingActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.WORK_HUNTING, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return  _hasEnoughResource(ResourceHandler.TOOLS, ACTION_CONSUME_TOOLS) && 
                _hasResourceCapacity(ResourceHandler.RAW_VENISON, ACTION_CREATE_RAW_VENISON) && 
                _hasFreeSpot(LocationTypes.HUNTING_SPOT) /*&&
                _npc.VillageInfo.WasHuntingGameKilled*/;
    }

    protected override void _addPlanActions()
    {
        float seconds = TimeController.gameSecondsFromRealHours(5.5f);

        _moveAndWaitRealSeconds(seconds, LocationTypes.HUNTING_SPOT, false,
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
        _resourceHandler.addResource(ACTION_CREATE_RAW_VENISON, ResourceHandler.RAW_VENISON);
        _npcState.gainMoney(LOW_CLASS_WORK_MONEY_REWARD);
    }
}
