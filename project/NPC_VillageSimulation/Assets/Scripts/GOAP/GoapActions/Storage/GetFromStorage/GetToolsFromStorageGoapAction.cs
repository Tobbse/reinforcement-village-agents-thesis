using SwordGC.AI.Goap;

public class GetToolsFromStorageGoapAction : BaseGetFromStorageGoapAction
{
    public GetToolsFromStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _effectType = Effects.HAS_TOOL_FOR_WORK;
        _locationType = LocationTypes.STORAGE_MAIN;
        _resourceType = ResourceHandler.TOOLS;
        _neededResourceAmount = 1;
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new GetToolsFromStorageGoapAction(this.agent, _npc);
    }
}