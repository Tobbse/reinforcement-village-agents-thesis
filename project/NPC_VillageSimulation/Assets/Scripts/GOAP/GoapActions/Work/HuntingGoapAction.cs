using SwordGC.AI.Goap;

public class HuntingGoapAction : BaseWorkGoapAction
{
    public HuntingGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _jobId = JobHandler.JOB_HUNTER;
        _locationType = LocationTypes.HUNTING_SPOT;

        _usedResourceType = null;
        _resourceNeededEffect = null;

        _addedResourceType = ResourceHandler.RAW_VENISON;
        _addedResourceAmount = WorkMeatHuntingActionPlan.ACTION_CREATE_RAW_VENISON;
        _resourceGainedEffect = Effects.HAS_VENISON_FOR_STORAGE;
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
        return new HuntingGoapAction(this.agent, _npc);
    }
}