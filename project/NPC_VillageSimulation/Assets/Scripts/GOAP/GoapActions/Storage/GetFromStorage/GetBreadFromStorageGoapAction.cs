using SwordGC.AI.Goap;

public class GetBreadFromStorageGoapAction : BaseGetFromStorageGoapAction
{
    public GetBreadFromStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _effectType = Effects.HAS_BREAD_FOR_WORK;
        _locationType = LocationTypes.STORAGE_MAIN;
        _resourceType = ResourceHandler.BREAD;
        _neededResourceAmount = 0; // Not actually used by the bread guy, but by the guests.
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new GetBreadFromStorageGoapAction(this.agent, _npc);
    }
}