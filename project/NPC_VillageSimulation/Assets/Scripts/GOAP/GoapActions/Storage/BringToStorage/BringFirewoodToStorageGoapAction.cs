using SwordGC.AI.Goap;

public class BringFirewoodToStorageGoapAction : BaseBringToStorageGoapAction
{
    public BringFirewoodToStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _locationType = LocationTypes.STORAGE_MAIN;
        _carriesResourceEffectType = Effects.HAS_FIREWOOD_FOR_STORAGE;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BringFirewoodToStorageGoapAction(this.agent, _npc);
    }
}