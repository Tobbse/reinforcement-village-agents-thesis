using SwordGC.AI.Goap;

public class RecreationSoccerGoapAction : BaseNpcGoapAction
{
    public RecreationSoccerGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        goal = GoapGoal.Goals.RecreationGoapGoal;
        preconditions.Add(Effects.NEEDS_RECREATION, true);
        effects.Add(Effects.NEEDS_RECREATION, false);
    }

    protected override void _init()
    {
        _locationType = LocationTypes.SOCCER_FIELD;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        _waitForIngameSeconds(BaseActionPlan.DEFAULT_ACTION_SECONDS);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new RecreationSoccerGoapAction(this.agent, _npc);
    }
}