using SwordGC.AI.Goap;

public class RecreationTavernGoapAction : BaseNpcGoapAction
{
    public RecreationTavernGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        goal = GoapGoal.Goals.RecreationGoapGoal;
        preconditions.Add(Effects.NEEDS_RECREATION, true);
        effects.Add(Effects.NEEDS_RECREATION, false);
    }

    protected override void _init()
    {
        _locationType = LocationTypes.TAVERN;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        _waitForIngameSeconds(BaseActionPlan.DEFAULT_ACTION_SECONDS);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new RecreationTavernGoapAction(this.agent, _npc);
    }
}