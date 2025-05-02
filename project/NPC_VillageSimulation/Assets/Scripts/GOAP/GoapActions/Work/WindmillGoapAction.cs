using SwordGC.AI.Goap;

public class WindmillGoapAction : BaseWorkGoapAction
{
    public WindmillGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _jobId = JobHandler.JOB_MILLER;
        _locationType = LocationTypes.WINDMILL;

        _usedResourceType = ResourceHandler.GRAIN;
        _resourceNeededEffect = Effects.HAS_GRAIN_FOR_WORK;

        _addedResourceType = ResourceHandler.FLOUR;
        _addedResourceAmount = WorkBreadWindmillActionPlan.ACTION_CREATE_FLOUR;
        _resourceGainedEffect = Effects.HAS_FLOUR_FOR_STORAGE;
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
        return new WindmillGoapAction(this.agent, _npc);
    }
}