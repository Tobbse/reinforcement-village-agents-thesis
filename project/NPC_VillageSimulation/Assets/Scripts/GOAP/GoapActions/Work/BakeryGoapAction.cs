using SwordGC.AI.Goap;

public class BakeryGoapAction : BaseWorkGoapAction
{
    public BakeryGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _jobId = JobHandler.JOB_BAKER;
        _locationType = LocationTypes.BAKERY;

        _usedResourceType = ResourceHandler.FLOUR;
        _resourceNeededEffect = Effects.HAS_FLOUR_FOR_WORK;

        _addedResourceType = ResourceHandler.BREAD;
        _addedResourceAmount = WorkBreadBakeryActionPlan.ACTION_CREATE_BREAD;
        _resourceGainedEffect = Effects.HAS_BREAD_FOR_STORAGE;
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
        return new BakeryGoapAction(this.agent, _npc);
    }
}