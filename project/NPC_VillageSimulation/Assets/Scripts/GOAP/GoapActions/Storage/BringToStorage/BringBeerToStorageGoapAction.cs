using SwordGC.AI.Goap;

public class BringBeerToStorageGoapAction : BaseBringToStorageGoapAction
{
    public BringBeerToStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _locationType = LocationTypes.STORAGE_MAIN;
        _carriesResourceEffectType = Effects.HAS_BEER_FOR_STORAGE;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BringBeerToStorageGoapAction(this.agent, _npc);
    }
}