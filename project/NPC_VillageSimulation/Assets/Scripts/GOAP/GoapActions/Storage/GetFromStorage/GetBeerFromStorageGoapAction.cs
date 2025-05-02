using SwordGC.AI.Goap;

public class GetBeerFromStorageGoapAction : BaseGetFromStorageGoapAction
{
    public GetBeerFromStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _effectType = Effects.HAS_BEER_FOR_WORK;
        _locationType = LocationTypes.STORAGE_MAIN;
        _resourceType = ResourceHandler.BEER;
        _neededResourceAmount = 0; // Not actually used by the tavern guy, but by the guests.
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new GetBeerFromStorageGoapAction(this.agent, _npc);
    }
}