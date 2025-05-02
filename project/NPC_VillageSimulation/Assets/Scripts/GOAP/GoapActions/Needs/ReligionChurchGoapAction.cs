using SwordGC.AI.Goap;

public class ReligionChurchGoapAction : BaseNpcGoapAction
{
    public ReligionChurchGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        goal = GoapGoal.Goals.FaithGoapGoal;
        preconditions.Add(Effects.NEEDS_FAITH, true);
        effects.Add(Effects.NEEDS_FAITH, false);
    }

    protected override void _init()
    {
        _locationType = LocationTypes.CHURCH;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        _waitForIngameSeconds(BaseActionPlan.DEFAULT_ACTION_SECONDS);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new ReligionChurchGoapAction(this.agent, _npc);
    }
}