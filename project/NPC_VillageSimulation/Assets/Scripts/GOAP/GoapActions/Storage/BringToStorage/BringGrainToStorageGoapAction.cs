using SwordGC.AI.Goap;

public class BringGrainToStorageGoapAction : BaseBringToStorageGoapAction
{
    public BringGrainToStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _locationType = LocationTypes.STORAGE_GRAIN;
        _carriesResourceEffectType = Effects.HAS_GRAIN_FOR_STORAGE;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BringGrainToStorageGoapAction(this.agent, _npc);
    }
}