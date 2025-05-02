using SwordGC.AI.Goap;

public class ThirstGoapAction : BaseNpcGoapAction
{
    public ThirstGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        goal = GoapGoal.Goals.ThirstGoapGoal;
        preconditions.Add(Effects.IS_THIRSTY, true);
        effects.Add(Effects.IS_THIRSTY, false);
    }

    protected override void _init()
    {
        _locationType = LocationTypes.WELL;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        _waitForIngameSeconds(BaseActionPlan.DEFAULT_ACTION_SECONDS);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new ThirstGoapAction(this.agent, _npc);
    }
}