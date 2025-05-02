using SwordGC.AI.Goap;

public class BringToolsToStorageGoapAction : BaseBringToStorageGoapAction
{
    public BringToolsToStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _locationType = LocationTypes.STORAGE_MAIN;
        _carriesResourceEffectType = Effects.HAS_TOOL_FOR_STORAGE;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BringToolsToStorageGoapAction(this.agent, _npc);
    }
}