using SwordGC.AI.Goap;

public class CommunicationChatGoapAction : BaseNpcGoapAction
{
    public CommunicationChatGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        goal = GoapGoal.Goals.CommunicationGoapGoal;
        preconditions.Add(Effects.NEEDS_COMMUNICATION, true);
        effects.Add(Effects.NEEDS_COMMUNICATION, false);
    }

    protected override void _init()
    {
        _locationType = LocationTypes.MEETING_POINT;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        _waitForIngameSeconds(BaseActionPlan.DEFAULT_ACTION_SECONDS);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new CommunicationChatGoapAction(this.agent, _npc);
    }
}