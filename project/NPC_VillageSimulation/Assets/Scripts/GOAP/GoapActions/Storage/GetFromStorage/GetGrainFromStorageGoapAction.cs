using SwordGC.AI.Goap;

public class GetGrainFromStorageGoapAction : BaseGetFromStorageGoapAction
{
    public GetGrainFromStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _effectType = Effects.HAS_GRAIN_FOR_WORK;
        _locationType = LocationTypes.STORAGE_GRAIN;
        _resourceType = ResourceHandler.GRAIN;
        _neededResourceAmount = WorkBreadWindmillActionPlan.ACTION_CONSUME_GRAIN;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new GetGrainFromStorageGoapAction(this.agent, _npc);
    }
}