using SwordGC.AI.Goap;

public class BringFlourToStorageGoapAction : BaseBringToStorageGoapAction
{
    public BringFlourToStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _locationType = LocationTypes.STORAGE_MAIN;
        _carriesResourceEffectType = Effects.HAS_FLOUR_FOR_STORAGE;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BringFlourToStorageGoapAction(this.agent, _npc);
    }
}