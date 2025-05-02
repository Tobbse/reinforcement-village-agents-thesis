using SwordGC.AI.Goap;
using System;

public class CommunicationCafeGoapAction : BaseNpcGoapAction
{
    public CommunicationCafeGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        goal = GoapGoal.Goals.CommunicationGoapGoal;
        preconditions.Add(Effects.NEEDS_COMMUNICATION, true);
        effects.Add(Effects.NEEDS_COMMUNICATION, false);
    }

    protected override void _init()
    {
        _locationType = LocationTypes.CAFE;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        _waitForIngameSeconds(BaseActionPlan.DEFAULT_ACTION_SECONDS);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new CommunicationCafeGoapAction(this.agent, _npc);
    }
}