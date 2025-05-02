using SwordGC.AI.Goap;

public class LoggingGoapAction : BaseWorkGoapAction
{
    public LoggingGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _jobId = JobHandler.JOB_LOGGER;
        _locationType = LocationTypes.WOOD_CUTTING_SPOT;

        _usedResourceType = null;
        _resourceNeededEffect = null;

        _addedResourceType = ResourceHandler.WOODEN_LOG;
        _addedResourceAmount = WorkWoodLoggingActionPlan.ACTION_CREATE_WOODEN_LOGS;
        _resourceGainedEffect = Effects.HAS_LOGS_FOR_STORAGE;
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
        return new LoggingGoapAction(this.agent, _npc);
    }
}