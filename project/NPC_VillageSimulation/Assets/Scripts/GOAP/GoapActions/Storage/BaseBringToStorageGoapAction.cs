using System.Collections;
using SwordGC.AI.Goap;

public class BaseBringToStorageGoapAction : BaseNpcGoapAction
{
    protected string _carriesResourceEffectType;

    public BaseBringToStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        //preconditions.Add(_carriesResourceEffectType, true);

        effects.Add(_carriesResourceEffectType, false);

        cost = 50;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        foreach (string resource in _npc.NpcAgentGoapData.Carrying.Keys)
        {
            _npc.ResourceHandler.addResource(_npc.NpcAgentGoapData.removeResource(resource), resource);
        }
        _waitForIngameSeconds(1f);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BaseBringToStorageGoapAction(this.agent, _npc);
    }
}