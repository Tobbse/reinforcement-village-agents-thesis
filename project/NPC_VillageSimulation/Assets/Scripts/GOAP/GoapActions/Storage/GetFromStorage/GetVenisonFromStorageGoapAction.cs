using SwordGC.AI.Goap;

public class GetVenisonFromStorageGoapAction : BaseGetFromStorageGoapAction
{
    public GetVenisonFromStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _effectType = Effects.HAS_VENISON_FOR_WORK;
        _locationType = LocationTypes.STORAGE_FOREST;
        _resourceType = ResourceHandler.RAW_VENISON;
        _neededResourceAmount = WorkMeatButcherActionPlan.ACTION_CONSUME_RAW_VENISON;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new GetVenisonFromStorageGoapAction(this.agent, _npc);
    }
}