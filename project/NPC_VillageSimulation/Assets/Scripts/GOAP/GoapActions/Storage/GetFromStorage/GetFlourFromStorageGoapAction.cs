using SwordGC.AI.Goap;

public class GetFlourFromStorageGoapAction : BaseGetFromStorageGoapAction
{
    public GetFlourFromStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _effectType = Effects.HAS_FLOUR_FOR_WORK;
        _locationType = LocationTypes.STORAGE_MAIN;
        _resourceType = ResourceHandler.FLOUR;
        _neededResourceAmount = WorkBreadBakeryActionPlan.ACTION_CONSUME_FLOUR;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new GetFlourFromStorageGoapAction(this.agent, _npc);
    }
}