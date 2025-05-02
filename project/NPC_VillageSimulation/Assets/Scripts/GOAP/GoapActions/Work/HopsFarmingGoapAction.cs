using SwordGC.AI.Goap;

public class HopsFarmingGoapAction : BaseWorkGoapAction
{
    public HopsFarmingGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _jobId = JobHandler.JOB_HOPS_FARMER;
        _locationType = LocationTypes.FIELD_HOPS;

        _usedResourceType = null;
        _resourceNeededEffect = null;

        _addedResourceType = ResourceHandler.HOPS;
        _addedResourceAmount = WorkBeerFarmHopsActionPlan.ACTION_CREATE_HOPS;
        _resourceGainedEffect = Effects.HAS_HOPS_FOR_STORAGE;
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
        return new HopsFarmingGoapAction(this.agent, _npc);
    }
}