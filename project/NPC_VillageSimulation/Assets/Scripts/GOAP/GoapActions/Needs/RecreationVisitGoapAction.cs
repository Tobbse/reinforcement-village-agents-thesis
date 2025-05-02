using SwordGC.AI.Goap;

public class RecreationVisitGoapAction : BaseNpcGoapAction
{
    public RecreationVisitGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        goal = GoapGoal.Goals.RecreationGoapGoal;
        preconditions.Add(Effects.NEEDS_RECREATION, true);
        effects.Add(Effects.NEEDS_RECREATION, false);
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
        return new RecreationVisitGoapAction(this.agent, _npc);
    }
}