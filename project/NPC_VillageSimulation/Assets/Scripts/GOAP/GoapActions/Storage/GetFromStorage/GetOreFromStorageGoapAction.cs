using SwordGC.AI.Goap;

public class GetOreFromStorageGoapAction : BaseGetFromStorageGoapAction
{
    public GetOreFromStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _effectType = Effects.HAS_ORE_FOR_WORK;
        _locationType = LocationTypes.STORAGE_MINE;
        _resourceType = ResourceHandler.ORE;
        _neededResourceAmount = WorkToolsBlacksmithActionPlan.ACTION_CONSUME_ORE;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new GetOreFromStorageGoapAction(this.agent, _npc);
    }
}