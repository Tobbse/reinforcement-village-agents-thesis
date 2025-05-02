using SwordGC.AI.Goap;

public class GetLogsFromStorageGoapAction : BaseGetFromStorageGoapAction
{
    public GetLogsFromStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _effectType = Effects.HAS_LOGS_FOR_WORK;
        _locationType = LocationTypes.STORAGE_FOREST;
        _resourceType = ResourceHandler.WOODEN_LOG;
        _neededResourceAmount = WorkWoodCarpenterActionPlan.ACTION_CONSUME_WOODEN_LOGS;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new GetLogsFromStorageGoapAction(this.agent, _npc);
    }
}