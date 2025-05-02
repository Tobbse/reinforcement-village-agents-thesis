using SwordGC.AI.Goap;

public class GrainFarmingGoapAction : BaseWorkGoapAction
{
    public GrainFarmingGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _jobId = JobHandler.JOB_GRAIN_FARMER;
        _locationType = LocationTypes.FIELD_GRAIN;

        _usedResourceType = null;
        _resourceNeededEffect = null;

        _addedResourceType = ResourceHandler.GRAIN;
        _addedResourceAmount = WorkBreadFarmGrainActionPlan.ACTION_CREATE_GRAIN;
        _resourceGainedEffect = Effects.HAS_GRAIN_FOR_STORAGE;
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
        return new GrainFarmingGoapAction(this.agent, _npc);
    }
}