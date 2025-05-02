using SwordGC.AI.Goap;

public class BreweryGoapAction : BaseWorkGoapAction
{
    public BreweryGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _jobId = JobHandler.JOB_BREWER;
        _locationType = LocationTypes.BREWERY;

        _usedResourceType = ResourceHandler.HOPS;
        _resourceNeededEffect = Effects.HAS_HOPS_FOR_WORK;

        _addedResourceType = ResourceHandler.BEER;
        _addedResourceAmount = WorkBeerBreweryActionPlan.ACTION_CREATE_BEER;
        _resourceGainedEffect = Effects.HAS_BEER_FOR_STORAGE;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        float seconds = TimeController.gameSecondsFromRealHours(4f);
        _waitForIngameSeconds(seconds);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BreweryGoapAction(this.agent, _npc);
    }
}