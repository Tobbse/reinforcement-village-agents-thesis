using System;

public class WorkBeerBreweryActionPlan : WorkActionPlan
{
    public const int ACTION_CREATE_BEER = 25;
    public const int ACTION_CONSUME_HOPS = 8;
    public const int ACTION_CONSUME_TOOLS = 1;

    public WorkBeerBreweryActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.WORK_BREWERY, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return  _hasEnoughResource(ResourceHandler.HOPS, ACTION_CONSUME_HOPS) &&
                _hasEnoughResource(ResourceHandler.TOOLS, ACTION_CONSUME_TOOLS) &&
                _hasResourceCapacity(ResourceHandler.BEER, ACTION_CREATE_BEER) &&
                _hasFreeSpot(LocationTypes.BREWERY);
    }

    protected override void _addPlanActions()
    {
        float seconds = TimeController.gameSecondsFromRealHours(4f);

        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS, LocationTypes.BREWERY);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_HOPS, true);
        _moveAndWaitRealSeconds(5f, LocationTypes.BREWERY);
        _moveAndWaitRealSeconds(5f, LocationTypes.WELL, true);
        _moveAndWaitRealSeconds(seconds, LocationTypes.BREWERY);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_MAIN, true);
    }

    protected override void _initRun()
    {
        base._initRun();
        _resourceHandler.useResource(ACTION_CONSUME_HOPS, ResourceHandler.HOPS);
        _resourceHandler.useResource(ACTION_CONSUME_TOOLS, ResourceHandler.TOOLS);
    }

    protected override void _additionalFinishAction()
    {
        _resourceHandler.addResource(ACTION_CREATE_BEER, ResourceHandler.BEER);
        _npcState.gainMoney(HIGH_CLASS_WORK_MONEY_REWARD);
    }
}
