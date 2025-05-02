using SwordGC.AI.Goap;

public class BringOreToStorageGoapAction : BaseBringToStorageGoapAction
{
    public BringOreToStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _locationType = LocationTypes.STORAGE_MINE;
        _carriesResourceEffectType = Effects.HAS_ORE_FOR_STORAGE;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BringOreToStorageGoapAction(this.agent, _npc);
    }
}