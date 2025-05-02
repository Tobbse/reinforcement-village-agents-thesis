using SwordGC.AI.Goap;

public class BringVenisonToStorageGoapAction : BaseBringToStorageGoapAction
{
    public BringVenisonToStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _locationType = LocationTypes.STORAGE_MAIN;
        _carriesResourceEffectType = Effects.HAS_VENISON_FOR_STORAGE;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BringVenisonToStorageGoapAction(this.agent, _npc);
    }
}