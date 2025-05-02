using SwordGC.AI.Goap;

public class CarpenterGoapAction : BaseWorkGoapAction
{
    public CarpenterGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _jobId = JobHandler.JOB_CARPENTER;
        _locationType = LocationTypes.CARPENTER;

        _usedResourceType = ResourceHandler.WOODEN_LOG;
        _resourceNeededEffect = Effects.HAS_LOGS_FOR_WORK;

        _addedResourceType = ResourceHandler.FIREWOOD;
        _addedResourceAmount = WorkWoodCarpenterActionPlan.ACTION_CREATE_FIREFOOD;
        _resourceGainedEffect = Effects.HAS_FIREWOOD_FOR_STORAGE;
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
        return new CarpenterGoapAction(this.agent, _npc);
    }
}