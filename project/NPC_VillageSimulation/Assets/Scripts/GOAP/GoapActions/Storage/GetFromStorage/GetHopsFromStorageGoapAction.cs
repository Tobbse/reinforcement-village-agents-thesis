using SwordGC.AI.Goap;

public class GetHopsFromStorageGoapAction : BaseGetFromStorageGoapAction
{
    public GetHopsFromStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _effectType = Effects.HAS_HOPS_FOR_WORK;
        _locationType = LocationTypes.STORAGE_HOPS;
        _resourceType = ResourceHandler.HOPS;
        _neededResourceAmount = WorkBeerBreweryActionPlan.ACTION_CONSUME_HOPS;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new GetHopsFromStorageGoapAction(this.agent, _npc);
    }
}