using System;

public class SleepActionPlan : BaseActionPlan
{
    public const float SLEEP_DURATION_HOURS = 5f;

    private float _initSleep;

    public SleepActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
    : base(NpcActions.SLEEP, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        return true;
    }

    protected override void _addPlanActions()
    {
        float seconds = TimeController.gameSecondsFromRealHours(5f);

        MoveAction moveAction = new MoveAction(_actionType, _npc, LocationTypes.HOME, true, MoveAction.PICK_BUILDING_STRATEGY_HOME);
        StartSleepingAction startSleepingAction = new StartSleepingAction(_actionType, _npc);
        WaitAction waitAction = new WaitAction(_actionType, _npc, seconds);
        StopSleepingAction stopSleepingAction = new StopSleepingAction(_actionType, _npc);

        _addAction(moveAction);
        _addAction(startSleepingAction);
        _addAction(waitAction);
        _addAction(stopSleepingAction);
    }

    protected override void _initRun()
    {
        base._initRun();
        _initSleep = _npc.NpcState.Sleep;
    }

    protected override float _getReward()
    {
        float sleepReward = 0.5f - _initSleep;
        return sleepReward * SLEEP_REWARD_MULTIPLIER;
    }

    protected override void _additionalFinishAction()
    {
        _npcState.updateExhaustion(1f);
    }
}
