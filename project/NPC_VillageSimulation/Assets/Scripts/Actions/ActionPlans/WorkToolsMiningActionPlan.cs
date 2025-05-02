using System;

public class WorkToolsMiningActionPlan : WorkActionPlan
{
    public const int ACTION_CREATE_ORE = 3;

    public WorkToolsMiningActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.WORK_MINING, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return  _hasResourceCapacity(ResourceHandler.ORE, ACTION_CREATE_ORE) &&
                _hasFreeSpot(LocationTypes.MINE);
    }

    protected override void _addPlanActions()
    {
        float seconds = TimeController.gameSecondsFromRealHours(4.5f);

        _moveAndWaitRealSeconds(2f, LocationTypes.STORAGE_MINE, true, MoveAction.PICK_BUILDING_STRATEGY_RANDOM);
        _moveAndWaitRealSeconds(seconds, LocationTypes.MINE);
        _moveAndWaitRealSeconds(5f, LocationTypes.STORAGE_MAIN, true);
    }

    protected override void _additionalFinishAction()
    {
        _resourceHandler.addResource(ACTION_CREATE_ORE, ResourceHandler.ORE);
        _npcState.gainMoney(LOW_CLASS_WORK_MONEY_REWARD);
    }
}
