using SwordGC.AI.Goap;

public class GetMeatFromStorageGoapAction : BaseGetFromStorageGoapAction
{
    public GetMeatFromStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _effectType = Effects.HAS_MEAT_FOR_WORK;
        _locationType = LocationTypes.STORAGE_MAIN;
        _resourceType = ResourceHandler.COOKED_MEAT;
        _neededResourceAmount = 0; // Not actually used by the butcher, but by the eating npcs.
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new GetMeatFromStorageGoapAction(this.agent, _npc);
    }
}