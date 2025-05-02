using SwordGC.AI.Goap;

public class HungerMeatGoapAction : BaseNpcGoapAction
{
    public HungerMeatGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        goal = GoapGoal.Goals.HungerGoapGoal;
        preconditions.Add(Effects.IS_HUNGRY, true);
        effects.Add(Effects.IS_HUNGRY, false);
    }

    protected override void _init()
    {
        _locationType = LocationTypes.MARKET;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        _waitForIngameSeconds(BaseActionPlan.DEFAULT_ACTION_SECONDS);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new HungerMeatGoapAction(this.agent, _npc);
    }
}