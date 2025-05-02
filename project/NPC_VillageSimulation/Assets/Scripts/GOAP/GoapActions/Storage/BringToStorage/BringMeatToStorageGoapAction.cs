using SwordGC.AI.Goap;

public class BringMeatToStorageGoapAction : BaseBringToStorageGoapAction
{
    public BringMeatToStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _locationType = LocationTypes.STORAGE_MAIN;
        _carriesResourceEffectType = Effects.HAS_MEAT_FOR_STORAGE;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BringMeatToStorageGoapAction(this.agent, _npc);
    }
}