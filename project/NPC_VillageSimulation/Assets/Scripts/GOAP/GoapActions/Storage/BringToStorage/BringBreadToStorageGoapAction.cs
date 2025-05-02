using SwordGC.AI.Goap;

public class BringBreadToStorageGoapAction : BaseBringToStorageGoapAction
{
    public BringBreadToStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _locationType = LocationTypes.STORAGE_MAIN;
        _carriesResourceEffectType = Effects.HAS_BREAD_FOR_STORAGE;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BringBreadToStorageGoapAction(this.agent, _npc);
    }
}