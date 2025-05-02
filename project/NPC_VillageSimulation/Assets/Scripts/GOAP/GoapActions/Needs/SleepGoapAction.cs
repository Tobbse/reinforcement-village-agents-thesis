using SwordGC.AI.Goap;

public class SleepGoapAction : BaseNpcGoapAction
{
    public SleepGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        goal = GoapGoal.Goals.SleepGoapGoal;
        preconditions.Add(Effects.NEEDS_SLEEP, true);
        effects.Add(Effects.NEEDS_SLEEP, false);
    }

    protected override void _init()
    {
        _locationType = LocationTypes.HOME;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        float seconds = TimeController.gameSecondsFromRealHours(
            SleepActionPlan.SLEEP_DURATION_HOURS);
        _waitForIngameSeconds(seconds);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new SleepGoapAction(this.agent, _npc);
    }
}