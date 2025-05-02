using SwordGC.AI.Goap;

public class EducationSchoolGoapAction : BaseNpcGoapAction
{
    public EducationSchoolGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        goal = GoapGoal.Goals.EducationGoapGoal;
        preconditions.Add(Effects.NEEDS_EDUCATION, true);
        effects.Add(Effects.NEEDS_EDUCATION, false);
    }

    protected override void _init()
    {
        _locationType = LocationTypes.SCHOOL;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        _waitForIngameSeconds(BaseActionPlan.DEFAULT_ACTION_SECONDS);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new EducationSchoolGoapAction(this.agent, _npc);
    }
}