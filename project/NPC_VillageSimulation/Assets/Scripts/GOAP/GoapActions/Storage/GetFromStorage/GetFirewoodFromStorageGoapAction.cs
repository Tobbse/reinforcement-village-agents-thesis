using SwordGC.AI.Goap;

public class GetFirewoodFromStorageGoapAction : BaseGetFromStorageGoapAction
{
    public GetFirewoodFromStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _effectType = Effects.HAS_FIREWOOD_FOR_WORK;
        _locationType = LocationTypes.STORAGE_MAIN;
        _resourceType = ResourceHandler.FIREWOOD;
        _neededResourceAmount = 0; // Not actually used by the carpenter, but by all npcs.
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new GetFirewoodFromStorageGoapAction(this.agent, _npc);
    }
}