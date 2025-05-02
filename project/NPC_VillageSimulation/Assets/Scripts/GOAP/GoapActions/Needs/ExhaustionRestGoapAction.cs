using SwordGC.AI.Goap;

public class ExhaustionRestGoapAction : BaseNpcGoapAction
{
    public ExhaustionRestGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        goal = GoapGoal.Goals.ExhaustionGoapGoal;
        preconditions.Add(Effects.NEEDS_EXHAUSTION, true);
        effects.Add(Effects.NEEDS_EXHAUSTION, false);
    }

    protected override void _init()
    {
        _locationType = LocationTypes.HOME;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        _waitForIngameSeconds(BaseActionPlan.DEFAULT_ACTION_SECONDS * 2f);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new ExhaustionRestGoapAction(this.agent, _npc);
    }
}