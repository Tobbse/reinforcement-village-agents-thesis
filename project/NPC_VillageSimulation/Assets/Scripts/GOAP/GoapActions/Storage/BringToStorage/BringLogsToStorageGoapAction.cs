using SwordGC.AI.Goap;

public class BringLogsToStorageGoapAction : BaseBringToStorageGoapAction
{
    public BringLogsToStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _locationType = LocationTypes.STORAGE_FOREST;
        _carriesResourceEffectType = Effects.HAS_LOGS_FOR_STORAGE;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BringLogsToStorageGoapAction(this.agent, _npc);
    }
}