using SwordGC.AI.Goap;

public class MiningGoapAction : BaseWorkGoapAction
{
    public MiningGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _jobId = JobHandler.JOB_MINER;
        _locationType = LocationTypes.MINE;

        _usedResourceType = null;
        _resourceNeededEffect = null;

        _addedResourceType = ResourceHandler.ORE;
        _addedResourceAmount = WorkToolsMiningActionPlan.ACTION_CREATE_ORE;
        _resourceGainedEffect = Effects.HAS_ORE_FOR_STORAGE;
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
        return new MiningGoapAction(this.agent, _npc);
    }
}