using SwordGC.AI.Goap;

public class BringHopsToStorageGoapAction : BaseBringToStorageGoapAction
{
    public BringHopsToStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _locationType = LocationTypes.FIELD_HOPS;
        _carriesResourceEffectType = Effects.HAS_HOPS_FOR_STORAGE;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BringHopsToStorageGoapAction(this.agent, _npc);
    }
}