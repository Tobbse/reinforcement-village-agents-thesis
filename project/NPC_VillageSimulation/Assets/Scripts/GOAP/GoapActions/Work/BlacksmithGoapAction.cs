using SwordGC.AI.Goap;

public class BlacksmithGoapAction : BaseWorkGoapAction
{
    public BlacksmithGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _jobId = JobHandler.JOB_BLACKSMITH;
        _locationType = LocationTypes.BLACKSMITH;

        _usedResourceType = ResourceHandler.ORE;
        _resourceNeededEffect = Effects.HAS_ORE_FOR_WORK;

        _addedResourceType = ResourceHandler.TOOLS;
        _addedResourceAmount = WorkToolsBlacksmithActionPlan.ACTION_CREATE_TOOLS;
        _resourceGainedEffect = Effects.HAS_TOOL_FOR_STORAGE;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        float seconds = TimeController.gameSecondsFromRealHours(5f);
        _waitForIngameSeconds(seconds);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BlacksmithGoapAction(this.agent, _npc);
    }
}